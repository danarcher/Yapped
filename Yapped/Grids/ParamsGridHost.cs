using System;
using System.Collections.Generic;
using System.Drawing;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the leftmost "params" grid.
    /// </summary>
    internal class ParamsGridHost : GridHost<ParamRoot>
    {
        private readonly History history;
        private readonly GridSet grids;

        public ParamsGridHost(History history, GridSet grids)
        {
            this.history = history;
            this.grids = grids;
        }

        public override int ColumnCount => 1;
        public override int RowCount => DataSource?.Count ?? 0;

        public override void Initialize(Grid grid)
        {
            grid.SelectedRowIndex = history.Current.Params.Selected;
            grid.SelectedColumnIndex = 0;
            grid.ScrollToSelection(GridScrollType.Center);
            base.Initialize(grid);
            InitializeRowsGrid();
        }

        public override string GetCellDisplayValue(int rowIndex, int columnIndex) => DataSource[rowIndex].Name;
        public override string GetCellToolTip(int rowIndex, int columnIndex) => DataSource[rowIndex].Description;
        public override string GetColumnName(int columnIndex) => "Name";
        public override int GetColumnWidth(Grid grid, int columnIndex) => grid.ClientSize.Width;

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (Initialized)
            {
                history.Current.Params.Selected = selectedRowIndex;
                InitializeRowsGrid();
            }
        }

        private void InitializeRowsGrid()
        {
            var selectedRowIndex = grids.Params.SelectedRowIndex;
            grids.RowsHost.ResetDataSource(selectedRowIndex >= 0 ? DataSource[selectedRowIndex] : null);
            // Since the user has changed the param they're viewing, scroll the selected cell into view.
            grids.Cells.ScrollToSelection(GridScrollType.Center);
        }

        public override void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style)
        {
            base.ModifyCellStyle(rowIndex, columnIndex, style);

            var param = DataSource[rowIndex];
            if (param.Error && !style.SelectedRow)
            {
                style.BackColor = Color.Pink;
                style.ForeColor = Color.Black;
            }
        }

        public override bool IsLinkClickable(int rowIndex, int columnIndex) => true;

        public override bool IsClickAlwaysLinkClick(int rowIndex, int columnIndex) => true;

        public override void LinkClicked(int rowIndex, int columnIndex)
        {
            if (rowIndex == grids.Params.SelectedRowIndex)
            {
                // Since mouse clicks count as link clicks, don't push duplicates to history.
                return;
            }
            history.Push(quiet: true);
            grids.Params.SelectedRowIndex = rowIndex;
            grids.Params.SelectedColumnIndex = columnIndex;
            grids.Params.ScrollToSelection(GridScrollType.Minimal);
        }
    }
}
