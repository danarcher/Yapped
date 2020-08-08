using System;
using System.Collections.Generic;
using System.Diagnostics;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the weapon damage calculator form's grid.
    /// </summary>
    internal class WeaponDamageGridHost : GridHost<List<WeaponDamage>>
    {
        private Grid grid;
        private int? sort = 2;

        public WeaponDamageGridHost(Grid grid)
        {
            this.grid = grid;
        }

        public override int ColumnCount => DamageType.Count + 3;
        public override int RowCount => DataSource?.Count ?? 0;

        public override string GetCellDisplayValue(int rowIndex, int columnIndex)
        {
            if (DataSource == null)
            {
                return string.Empty;
            }
            var item = DataSource[rowIndex];
            switch (columnIndex)
            {
                case 0:
                    return item.Name;
                case 1:
                    return item.TotalDamage.ToString();
                case int n when n >= 2 && n < DamageType.Count + 2:
                    return item.Damage[n - 2].ToString();
                case int n when n == DamageType.Count + 2:
                    return item.Buffable ? "✓" : string.Empty;
                default:
                    return string.Empty;
            }
        }

        public override string GetColumnName(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "Weapon";
                case 1:
                    return "Total AR";
                case int n when n >= 2 && n < DamageType.Count + 2:
                    return DamageType.Names[n - 2];
                case int n when n == DamageType.Count + 2:
                    return "Buffable";
                default:
                    return string.Empty;
            }
        }

        public override int GetColumnWidth(Grid grid, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return grid.Font.Height * 18;
                default:
                    return grid.Font.Height * 4;
            }
        }

        public override bool IsColumnClickable(int columnIndex) => true;

        public override void ColumnClicked(int columnIndex)
        {
            var n = columnIndex + 1;
            if (sort == n)
            {
                sort = -n;
            }
            else
            {
                sort = n;
            }
            Sort();
        }

        public void Sort()
        {
            Debug.Assert(DataSource.TrueForAll(x => x != null));
            switch (sort)
            {
                case int n when Math.Abs(n) == 1:
                    DataSource.Sort((a, b) => Math.Sign(n) * (a.Name ?? string.Empty).CompareTo(b.Name ?? string.Empty));
                    break;
                case int n when Math.Abs(n) == 2:
                    DataSource.Sort((a, b) => Math.Sign(n) * (b.TotalDamage - a.TotalDamage));
                    break;
                case int n when Math.Abs(n) >= 3 && Math.Abs(n) < DamageType.Count + 3:
                    DataSource.Sort((a, b) => Math.Sign(n) * (b.Damage[Math.Abs(n) - 3] - a.Damage[Math.Abs(n) - 3]));
                    break;
                case int n when Math.Abs(n) == DamageType.Count + 3:
                    DataSource.Sort((a, b) => Math.Sign(n) * b.Buffable.CompareTo(a.Buffable));
                    break;
                default:
                    break;
            }
            grid.Invalidate();            
        }
    }
}
