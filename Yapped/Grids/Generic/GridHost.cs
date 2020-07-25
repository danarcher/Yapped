using System;

namespace Yapped.Grids.Generic
{
    /// <summary>
    /// An optional base class for IGridHost-derived classes, implementing common functionality.
    /// </summary>
    public abstract class GridHost<T> : IGridHost
    {
        private T dataSource;

        public T DataSource
        {
            get => dataSource;
            set
            {
                if (!object.Equals(dataSource, value))
                {
                    dataSource = value;
                    DataSourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public abstract int ColumnCount { get; }
        public abstract int RowCount { get; }

        public abstract string GetCellDisplayValue(int rowIndex, int columnIndex);
        public abstract string GetColumnName(int columnIndex);
        public abstract int GetColumnWidth(Grid grid, int columnIndex);

        public virtual void SelectionChanging(int selectedRowIndex, int selectedColumnIndex) { }
        public virtual void SelectionChanged(int selectedRowIndex, int selectedColumnIndex) { }
        public virtual void ScrollTopChanged(int scrollTop) { }

        public virtual void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style) { }

        public event EventHandler DataSourceChanged;
    }
}
