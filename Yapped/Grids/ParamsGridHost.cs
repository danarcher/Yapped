using System.Collections.Generic;
using System.Drawing;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// The grid host for the leftmost "params" grid.
    /// </summary>
    internal class ParamsGridHost : GridHost<IList<ParamWrapper>>
    {
        private bool initialized = false;
        private readonly SelectionMemory memory;
        private readonly Grid rowsGrid;
        private readonly Grid cellsGrid;

        public ParamsGridHost(SelectionMemory memory, Grid rowsGrid, Grid cellsGrid)
        {
            this.memory = memory;
            this.rowsGrid = rowsGrid;
            this.cellsGrid = cellsGrid;
        }

        public override int ColumnCount => 1;
        public override int RowCount => DataSource?.Count ?? 0;

        public override void Initialize(Grid grid)
        {
            grid.ScrollTop = memory.TopParam;
            grid.SelectedRowIndex = memory.SelectedParam;
            grid.SelectedColumnIndex = 0;
            initialized = true;
        }

        public override string GetCellDisplayValue(int rowIndex, int columnIndex) => DataSource[rowIndex].Name;
        public override string GetCellToolTip(int rowIndex, int columnIndex) => DataSource[rowIndex].Description;
        public override string GetColumnName(int columnIndex) => "Name";
        public override int GetColumnWidth(Grid grid, int columnIndex) => grid.ClientSize.Width;

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (selectedRowIndex >= 0)
            {
                // Remember the selected param.
                if (initialized)
                {
                    memory.SelectedParam = selectedRowIndex;
                }
                memory.StoreParamName(DataSource[selectedRowIndex].Name);
            }

            using (memory.WithoutStoringValues())
            {
                // Set up the rows grid for this param.
                rowsGrid.Host = new RowsGridHost(memory, cellsGrid)
                {
                    DataSource = selectedRowIndex >= 0 ? DataSource[selectedRowIndex] : null
                };

                // Restore its visual state.
                if (memory.SelectedRow.RecallValue(out int recall))
                {
                    rowsGrid.SelectedRowIndex = recall;
                    rowsGrid.SelectedColumnIndex = 1;
                    rowsGrid.ScrollToSelection();
                }
                else if (rowsGrid.RowCount > 0)
                {
                    rowsGrid.SelectedRowIndex = 0;
                    rowsGrid.SelectedColumnIndex = 1;
                }
                if (memory.TopRow.RecallValue(out int recallTop))
                {
                    rowsGrid.ScrollTop = recallTop;
                }
            }
        }

        public override void ScrollTopChanged(int scrollTop)
        {
            if (initialized)
            {
                memory.TopParam = scrollTop;
            }
        }

        public override void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style)
        {
            var param = DataSource[rowIndex];
            if (param.Error && !style.SelectedRow)
            {
                style.BackColor = Color.Pink;
                style.ForeColor = Color.Black;
            }
        }
    }
}
