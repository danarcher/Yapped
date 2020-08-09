using System.Linq;
using Yapped.Grids.Generic;
using CellType = SoulsFormats.PARAM.CellType;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the middle "rows" grid.
    /// </summary>
    internal class RowsGridHost : GridHost<ParamWrapper>
    {
        private readonly History history;
        private readonly GridSet grids;

        public RowsGridHost(History history, GridSet grids)
        {
            this.history = history;
            this.grids = grids;
        }

        public bool Initialized { get; set; }
        public override int ColumnCount => 2;
        public override int RowCount => DataSource?.Rows?.Count ?? 0;

        public override void Initialize(Grid grid)
        {
            grid.SelectedRowIndex = history.Current[history.Current.Params.Selected].Rows.Selected;
            grid.SelectedColumnIndex = 1;
            grid.ScrollToSelection();
            Initialized = true;
            InitializeCellsGrid();
        }

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
                    return grid.FontHeight * 5;
                case 1:
                    return grid.ClientSize.Width - grid.FontHeight * 4;
                default:
                    return 0;
            }
        }

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (Initialized)
            {
                history.Current[history.Current.Params.Selected].Rows.Selected = selectedRowIndex;
                InitializeCellsGrid();
            }
        }

        private void InitializeCellsGrid()
        {
            var selectedRowIndex = grids.Rows.SelectedRowIndex;
            grids.CellsHost.Initialized = false;
            grids.CellsHost.DataSource = selectedRowIndex >= 0 ? DataSource.Rows[selectedRowIndex].Cells.Where(cell => cell.Type != CellType.dummy8).ToArray() : null;
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
