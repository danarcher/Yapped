using System;
using System.Drawing;

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
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Set the data source. Always raises a changed event, regardless of the current value.
        /// </summary>
        public virtual void ResetDataSource(T value = default(T))
        {
            Initialized = false;
            dataSource = value;
            OnDataSourceChanged();
        }

        public bool Initialized { get; protected set; }

        public abstract int ColumnCount { get; }
        public abstract int RowCount { get; }

        public virtual void Initialize(Grid grid)
        {
            Initialized = true;
        }

        public abstract string GetCellDisplayValue(int rowIndex, int columnIndex);
        public virtual string GetCellToolTip(int rowIndex, int columnIndex) => string.Empty;

        public abstract string GetColumnName(int columnIndex);
        public abstract int GetColumnWidth(Grid grid, int columnIndex);

        public virtual void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style)
        {
            if (IsLinkClickable(rowIndex, columnIndex))
            {
                if (!style.SelectedCell)
                {
                    style.ForeColor = Color.Blue;
                }
                style.Underline = true;
            }
        }

        public virtual void SelectionChanging(int selectedRowIndex, int selectedColumnIndex) { }
        public virtual void SelectionChanged(int selectedRowIndex, int selectedColumnIndex) { }
        public virtual void ScrollTopChanged(int scrollTop) { }

        public virtual GridCellType GetCellEditType(int rowIndex, int columnIndex) => GridCellType.None;
        public virtual (string, object)[] GetCellEnumValues(int rowIndex, int columnIndex) => null;

        public virtual object GetCellEditValue(int rowIndex, int columnIndex) => null;
        public virtual void SetCellEditValue(int rowIndex, int columnIndex, object value) { }

        public virtual bool IsColumnClickable(int columnIndex) => false;
        public virtual void ColumnClicked(int columnIndex) { }

        public virtual bool IsLinkClickable(int rowIndex, int columnIndex) => false;
        public virtual bool IsClickAlwaysLinkClick(int rowIndex, int columnIndex) => false;
        public virtual void LinkClicked(int rowIndex, int columnIndex) { }

        public event EventHandler DataSourceChanged;

        protected virtual void OnDataSourceChanged()
        {
            DataSourceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
