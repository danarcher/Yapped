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
        private float stat0;
        private float stat1;
        private float stat2;
        private float stat3;
        private float stat4;

        private float result0;
        private float result1;
        private float result2;
        private float result3;
        private float result4;

        private float exponent0;
        private float exponent1;
        private float exponent2;
        private float exponent3;
        private float exponent4;

        public CalcCorrectGraph(ParamWrapper calcCorrectGraph, int statFuncId)
        {
            var item = calcCorrectGraph.Rows.First(x => x.ID == statFuncId);

            stat0 = (float)item["stageMaxVal0"].Value;
            stat1 = (float)item["stageMaxVal1"].Value;
            stat2 = (float)item["stageMaxVal2"].Value;
            stat3 = (float)item["stageMaxVal3"].Value;
            stat4 = (float)item["stageMaxVal4"].Value;

            result0 = (float)item["stageMaxGrowVal0"].Value;
            result1 = (float)item["stageMaxGrowVal1"].Value;
            result2 = (float)item["stageMaxGrowVal2"].Value;
            result3 = (float)item["stageMaxGrowVal3"].Value;
            result4 = (float)item["stageMaxGrowVal4"].Value;

            exponent0 = (float)item["adjPt_maxGrowVal0"].Value;
            exponent1 = (float)item["adjPt_maxGrowVal1"].Value;
            exponent2 = (float)item["adjPt_maxGrowVal2"].Value;
            exponent3 = (float)item["adjPt_maxGrowVal3"].Value;
            exponent4 = (float)item["adjPt_maxGrowVal4"].Value;
        }

        public float Apply(float stat)
        {
            if (stat == 0) return 0;
            if (stat < stat1)
            {
                return Calculate(stat, stat0, stat1, result0, result1, exponent0);
            }
            else if (stat < stat2)
            {
                return Calculate(stat, stat1, stat2, result1, result2, exponent1);
            }
            else if (stat < stat3)
            {
                return Calculate(stat, stat2, stat3, result2, result3, exponent2);
            }
            return Calculate(stat, stat3, stat4, result3, result4, exponent3);
            // TODO: exponent4 is used where?
        }

        private float Calculate(float stat, float statMin, float statMax, float resultMin, float resultMax, float exponent)
        {
            var statRange = statMax - statMin;
            var resultRange = resultMax - resultMin;

            var percent = Math.Max(0, Math.Min(statRange, stat - statMin)) / statRange;
            if (exponent >= 0)
            {
                percent = (float)Math.Pow(percent, exponent);
            }
            else
            {
                percent = 1 - (float)Math.Pow(1 - percent, -exponent);
            }
            return (resultMin + percent * resultRange) / (result4 == 0 ? 1 : result4);
        }
    }

}
