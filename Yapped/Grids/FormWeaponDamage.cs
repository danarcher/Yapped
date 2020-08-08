using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    internal partial class FormWeaponDamage : Form
    {
        private Grid grid;
        private WeaponDamageGridHost host;
        private List<WeaponDamage> results = new List<WeaponDamage>();

        public FormWeaponDamage()
        {
            InitializeComponent();
        }

        public ParamRoot Root { get; set; }
        public Font GridFont { get; set; }

        public static void ShowDialog(Font font, ParamRoot root)
        {
            using (var form = new FormWeaponDamage())
            {
                form.GridFont = font;
                form.Root = root;
                form.ShowDialog();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            grid = new Grid();
            panel.Controls.Add(grid);
            grid.Dock = DockStyle.Fill;
            grid.Font = GridFont;
            grid.Host = host = new WeaponDamageGridHost(grid) { DataSource = results }; ;
            ComputeWeaponDamage();
            grid.InvalidateDataSource();
            base.OnLoad(e);
        }

        private void UpDown_ValueChanged(object sender, EventArgs e)
        {
            //TryComputeWeaponDamage();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            TryComputeWeaponDamage();
        }

        private void TryComputeWeaponDamage()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                ComputeWeaponDamage();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ComputeWeaponDamage()
        {
            var equipParamWeapon = Root["EquipParamWeapon"];
            var reinforceParamWeapon = Root["ReinforceParamWeapon"];
            var attackElementCorrectParam = Root["AttackElementCorrectParam"];
            var calcCorrectGraph = Root["CalcCorrectGraph"];

            var stats = new int[]
            {
                (int)strengthUpDown.Value,
                (int)agilityUpDown.Value,
                (int)magicUpDown.Value,
                (int)faithUpDown.Value,
                (int)luckUpDown.Value,
            };

            results.Clear();
            foreach (var weapon in equipParamWeapon.Rows)
            {
                var calc = new WeaponDamageCalculator(weapon, reinforceParamWeapon, attackElementCorrectParam, calcCorrectGraph);
                var result = calc.Calculate(stats);
                results.Add(result);
            }
            host.Sort();
            grid.Invalidate();
        }

        private void UpDown_Enter(object sender, EventArgs e)
        {
            HighlightText((NumericUpDown)sender);
        }

        private void UpDown_MouseUp(object sender, MouseEventArgs e)
        {
            HighlightText((NumericUpDown)sender);
        }

        private static void HighlightText(NumericUpDown upDown) => upDown.Select(0, upDown.Text.Length);
    }
}
