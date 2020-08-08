using System.Collections.Generic;
using System.Linq;
using SoulsFormats;
using Yapped.Grids.Generic;
using CellType = SoulsFormats.PARAM.CellType;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the middle "rows" grid.
    /// </summary>
    internal class RowsGridHost : GridHost<ParamWrapper>
    {
        private SelectionMemory memory;
        private Grid cellsGrid;

        public RowsGridHost(SelectionMemory memory, Grid cellsGrid)
        {
            this.memory = memory;
            this.cellsGrid = cellsGrid;
        }

        public override int ColumnCount => 2;
        public override int RowCount => DataSource?.Rows?.Count ?? 0;

        public override string GetCellDisplayValue(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return DataSource.Rows[rowIndex].ID.ToString();
                case 1:
                    return DataSource.Rows[rowIndex].Name?.ToString() ?? string.Empty;
                default:
                    return string.Empty;
            }
        }

        public override string GetColumnName(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "ID";
                case 1:
                    return "Name";
                default:
                    return string.Empty;
            }
        }

        public override int GetColumnWidth(Grid grid, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return grid.Font.Height * 5;
                case 1:
                    return grid.ClientSize.Width - grid.Font.Height * 4;
                default:
                    return 0;
            }
        }

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (selectedRowIndex >= 0)
            {
                // Remember the selected row.
                memory.SelectedRow.StoreValue(selectedRowIndex);
            }

            using (memory.WithoutStoringValues())
            {
                // Set up the cells grid for this row.
                cellsGrid.Host = new CellsGridHost(memory)
                {
                    DataSource = selectedRowIndex >= 0 ? DataSource.Rows[selectedRowIndex].Cells.Where(cell => cell.Type != CellType.dummy8).ToArray() : null
                };

                // Restore its visual state.
                if (memory.SelectedCell.RecallValue(out int recall))
                {
                    cellsGrid.SelectedRowIndex = recall;
                    cellsGrid.SelectedColumnIndex = 1;
                    cellsGrid.ScrollToSelection();
                }
                else if (cellsGrid.RowCount > 0)
                {
                    cellsGrid.SelectedRowIndex = 0;
                    cellsGrid.SelectedColumnIndex = 1;
                }
                if (memory.TopCell.RecallValue(out int recallTop))
                {
                    cellsGrid.ScrollTop = recallTop;
                }
            }
        }

        public override void ScrollTopChanged(int scrollTop)
        {
            memory.TopRow.StoreValue(scrollTop);
        }

        public override GridCellType GetCellEditType(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return GridCellType.UInt32;
                case 1:
                    return GridCellType.String;
                default:
                    return GridCellType.None;
            }
        }

        public override object GetCellEditValue(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return (uint)DataSource.Rows[rowIndex].ID;
                case 1:
                    return DataSource.Rows[rowIndex].Name;
                default:
                    return null;
            }
        }

        public override void SetCellEditValue(int rowIndex, int columnIndex, object value)
        {
            switch (columnIndex)
            {
                case 0:
                    DataSource.Rows[rowIndex].ID = (uint)value;
                    break;
                case 1:
                    DataSource.Rows[rowIndex].Name = (string)value;
                    break;
            }
        }
    }
}
