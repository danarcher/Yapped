using System;
using System.Collections.Generic;
using System.Linq;
using SoulsFormats;

namespace Yapped
{
    internal static class DamageType
    {
        public const int Physical = 0;
        public const int Magic = 1;
        public const int Fire = 2;
        public const int Thunder = 3;
        public const int Dark = 4;
        public const int Count = 5;

        public static readonly string[] Names = new[] { "Physical", "Magic", "Fire", "Thunder", "Dark" };
    }

    internal static class Stat
    {
        public const int Strength = 0;
        public const int Agility = 1;
        public const int Magic = 2;
        public const int Faith = 3;
        public const int Luck = 4;
        public const int Count = 5;
    }

    internal class WeaponDamageCalculator
    {
        private static readonly string[] AttackBaseNames = new[] { "atkBasePhysics", "atkBaseMagic", "atkBaseFire", "atkBaseThunder", "atkBaseDark" };
        private static readonly string[] AttackRateNames = new[] { "physicsAtkRate", "magicAtkRate", "fireAtkRate", "thunderAtkRate", "darkAtkRate" };

        private static readonly string[] CorrectStatNames = new[] { "correctStrength", "correctAgility", "corretMagic", "corretFaith", "correctLuck" };
        private static readonly string[] CorrectRateNames = new[] { "correctStrengthRate", "correctAgilityRate", "correctMagicRate", "correctFaithRate", null };

        private static readonly string[] DamageGraphIdNames = new[] { "correctType", "Unk21", "Unk22", "Unk23", "Unk26" };

        public string BaseWeaponName;
        public string UpgradedWeaponName;
        public bool Buffable;

        public int ReinforceTypeId;
        public int MaxUpgrade;
        public PARAM.Row Reinforcement;

        public short[] AttackBase = new short[DamageType.Count];
        public float[] AttackRate = new float[DamageType.Count];

        public float[] CorrectStat = new float[Stat.Count];
        public float[] CorrectRate = new float[Stat.Count];

        public byte[] DamageGraphId = new byte[DamageType.Count];
        public CalcCorrectGraph[] DamageGraph = new CalcCorrectGraph[DamageType.Count];

        public int AttackElementCorrectId;
        public PARAM.Row AttackElementCorrect;
        public bool[,] AttackElement = new bool[DamageType.Count, Stat.Count];
        public short[,] ScaleElement = new short[DamageType.Count, Stat.Count];

        public float[] BaseDamage = new float[DamageType.Count];
        public float[] Damage = new float[DamageType.Count];

        public WeaponDamageCalculator(PARAM.Row weapon, ParamWrapper reinforceParamWeapon, ParamWrapper attackElementCorrectParam, ParamWrapper calcCorrectGraph)
        {
            BaseWeaponName = weapon.Name;
            Buffable = (bool)weapon["isEnhance"].Value;

            // Find out how this weapon is reinforced.
            ReinforceTypeId = (short)weapon["reinforceTypeId"].Value;

            // Find out how this weapon applies stat scaling to damage types.
            AttackElementCorrectId = (int)weapon["attackElementCorrectId"].Value;

            // Compute the weapon's maximum upgrade level.
            MaxUpgrade = 0;
            for (var i = 0; i <= 15; ++i)
            {
                if ((int)weapon.Cells.First(x => x.Name == $"originEquipWep{i}").Value >= 0)
                {
                    MaxUpgrade = i;
                }
            }

            // Load the relevant reinforcement.
            Reinforcement = reinforceParamWeapon.Rows.FirstOrDefault(x => x.ID == ReinforceTypeId + MaxUpgrade); // assume fully upgraded
            if (Reinforcement == null)
            {
                MaxUpgrade = 0;
                Reinforcement = reinforceParamWeapon.Rows.First(x => x.ID == ReinforceTypeId); // presume no upgrade path
            }

            // Name the upgraded weapon.
            UpgradedWeaponName = (MaxUpgrade == 0) ? BaseWeaponName : $"{BaseWeaponName} +{MaxUpgrade}";

            // Load the stat scaling to damage application table.
            AttackElementCorrect = attackElementCorrectParam.Rows.FirstOrDefault(x => x.ID == AttackElementCorrectId);
            if (AttackElementCorrect == null)
            {
                // Default to standard? Some rows missing e.g. Demon's Axe, Cinders mod.
                AttackElementCorrect = attackElementCorrectParam.Rows.First(x => x.ID == 10000);
            }

            // Load data per damage type.
            for (var i = 0; i < DamageType.Count; ++i)
            {
                AttackBase[i] = (short)weapon[AttackBaseNames[i]].Value;
                AttackRate[i] = (float)Reinforcement[AttackRateNames[i]].Value;
                DamageGraphId[i] = (byte)weapon[DamageGraphIdNames[i]].Value;
                DamageGraph[i] = new CalcCorrectGraph(calcCorrectGraph, DamageGraphId[i]);

                for (var j = 0; j < Stat.Count; ++j)
                {
                    AttackElement[i, j] = (bool)AttackElementCorrect.Cells[i * Stat.Count + j].Value;
                    ScaleElement[i, j] = (short)AttackElementCorrect[$"corrRate{i * Stat.Count + j}"].Value;
                }
            }

            // Load data per stat type.
            for (var i = 0; i < Stat.Count; ++i)
            {
                CorrectStat[i] = (float)weapon[CorrectStatNames[i]].Value;
                CorrectRate[i] = CorrectRateNames[i] != null ? (float)Reinforcement[CorrectRateNames[i]].Value : 1.0f;
            }
        }

        public WeaponDamage Calculate(int[] stats)
        {
            var result = new WeaponDamage();
            result.Name = UpgradedWeaponName;

            for (var damageType = 0; damageType < DamageType.Count; ++damageType)
            {
                BaseDamage[damageType] = AttackBase[damageType] * AttackRate[damageType];
                Damage[damageType] = BaseDamage[damageType];

                for (var stat = 0; stat < Stat.Count; ++stat)
                {
                    if (AttackElement[damageType, stat])
                    {
                        var rating = DamageGraph[damageType].Apply(stats[stat]);
                        var scaling = CorrectStat[stat] / 100.0f * CorrectRate[stat];
                        var multiplier = ScaleElement[damageType, stat] / 100.0f;
                        var bonus = BaseDamage[damageType] * scaling * rating * multiplier;
                        Damage[damageType] += bonus;
                    }
                }

                result.Damage[damageType] = (int)Damage[damageType];
                result.TotalDamage += result.Damage[damageType];
                result.Buffable = Buffable;
            }

            return result;
        }
    }

    internal class WeaponDamage
    {
        public string Name;
        public int[] Damage = new int[DamageType.Count];
        public int TotalDamage;
        public bool Buffable;
    }

    // c/o https://github.com/tzbob/ds3-ar/blob/master/shared/src/main/scala/ds3ar/ir/CalcCorrectGraph.scala
    internal class CalcCorrectGraph
    {
        private float stageMaxVal0;
        private float stageMaxVal1;
        private float stageMaxVal2;
        private float stageMaxVal3;
        private float stageMaxVal4;

        private float stageMaxGrowVal0;
        private float stageMaxGrowVal1;
        private float stageMaxGrowVal2;
        private float stageMaxGrowVal3;
        private float stageMaxGrowVal4;

        private float adjPtMaxGrowVal0;
        private float adjPtMaxGrowVal1;
        private float adjPtMaxGrowVal2;
        private float adjPtMaxGrowVal3;
        private float adjPtMaxGrowVal4;

        public CalcCorrectGraph(ParamWrapper calcCorrectGraph, int statFuncId)
        {
            var item = calcCorrectGraph.Rows.First(x => x.ID == statFuncId);

            stageMaxVal0 = (float)item["stageMaxVal0"].Value;
            stageMaxVal1 = (float)item["stageMaxVal1"].Value;
            stageMaxVal2 = (float)item["stageMaxVal2"].Value;
            stageMaxVal3 = (float)item["stageMaxVal3"].Value;
            stageMaxVal4 = (float)item["stageMaxVal4"].Value;

            stageMaxGrowVal0 = (float)item["stageMaxGrowVal0"].Value;
            stageMaxGrowVal1 = (float)item["stageMaxGrowVal1"].Value;
            stageMaxGrowVal2 = (float)item["stageMaxGrowVal2"].Value;
            stageMaxGrowVal3 = (float)item["stageMaxGrowVal3"].Value;
            stageMaxGrowVal4 = (float)item["stageMaxGrowVal4"].Value;

            adjPtMaxGrowVal0 = (float)item["adjPt_maxGrowVal0"].Value;
            adjPtMaxGrowVal1 = (float)item["adjPt_maxGrowVal1"].Value;
            adjPtMaxGrowVal2 = (float)item["adjPt_maxGrowVal2"].Value;
            adjPtMaxGrowVal3 = (float)item["adjPt_maxGrowVal3"].Value;
            adjPtMaxGrowVal4 = (float)item["adjPt_maxGrowVal4"].Value;
        }

        public float Apply(float stat)
        {
            if (stat == 0) return 0;
            if (stat < stageMaxVal0)
            {
                return Calculate(stat, stageMaxVal0, stageMaxVal1, stageMaxGrowVal0, stageMaxGrowVal1, adjPtMaxGrowVal0);
            }
            else if (stat < stageMaxVal1)
            {
                return Calculate(stat, stageMaxVal1, stageMaxVal2, stageMaxGrowVal1, stageMaxGrowVal2, adjPtMaxGrowVal1);
            }
            else if (stat < stageMaxVal2)
            {
                return Calculate(stat, stageMaxVal2, stageMaxVal3, stageMaxGrowVal2, stageMaxGrowVal3, adjPtMaxGrowVal2);
            }
            return Calculate(stat, stageMaxVal3, stageMaxVal4, stageMaxGrowVal3, stageMaxGrowVal4, adjPtMaxGrowVal3);
        }

        private float Calculate(float stat, float xA, float xB, float yA, float yB, float p)
        {
            var dx = (stat - xA) / (xB - xA);
            var fdx = (p > 0) ? (float)Math.Pow(dx, p) : 1 - (float)Math.Pow(1 - dx, -p);
            return (yA + fdx * (yB - yA)) / 100.0f;
        }
    }

}
