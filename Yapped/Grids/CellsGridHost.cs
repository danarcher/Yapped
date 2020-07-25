using System.Drawing;
using SoulsFormats;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the rightmost "cells" grid.
    /// </summary>
    internal class CellsGridHost : GridHost<PARAM.Cell[]>
    {
        private readonly SelectionMemory memory;

        public CellsGridHost(SelectionMemory memory)
        {
            this.memory = memory;
        }

        public override int ColumnCount => 3;

        public override int RowCount => DataSource?.Length ?? 0;

        public override string GetCellDisplayValue(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return DataSource[rowIndex].Type.ToString();
                case 1:
                    return DataSource[rowIndex].Name?.ToString() ?? string.Empty;
                case 2:
                    return DataSource[rowIndex].Value?.ToString() ?? string.Empty;
                default:
                    return string.Empty;
            }
        }

        public override string GetColumnName(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "Type";
                case 1:
                    return "Name";
                case 2:
                    return "Value";
                default:
                    return string.Empty;
            }
        }

        public override int GetColumnWidth(Grid grid, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return 50;
                case 1:
                    return 400;
                case 2:
                    return grid.ClientSize.Width - 450;
                default:
                    return 0;
            }
        }

        public override void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style)
        {
            // Highlight non-default cell values.
            var cell = DataSource[rowIndex];
            if (!object.Equals(cell.Value, cell.Layout.Default))
            {
                if (!style.SelectedRow)
                {
                    style.BackColor = Color.FromArgb(255, 240, 220);
                    style.ForeColor = Color.Brown;
                }
                style.Bold = true;
            }
        }

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (selectedRowIndex >= 0)
            {
                memory.SelectedCell.StoreValue(selectedRowIndex);
            }
        }

        public override void ScrollTopChanged(int scrollTop)
        {
            memory.TopCell.StoreValue(scrollTop);
        }
    }
}
