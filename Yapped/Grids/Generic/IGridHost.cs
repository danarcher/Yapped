﻿using System;

namespace Yapped.Grids.Generic
{
    /// <summary>
    /// Interface to a grid host, which determines the data shown in a grid.
    /// </summary>
    public interface IGridHost
    {
        int ColumnCount { get; }
        int RowCount { get; }

        string GetColumnName(int columnIndex);
        int GetColumnWidth(Grid grid, int olumnIndex);

        string GetCellDisplayValue(int rowIndex, int columnIndex);
        string GetCellToolTip(int rowIndex, int columnIndex);
        void ModifyCellStyle(int rowIndex, int columnIndex, GridCellStyle style);

        void SelectionChanging(int selectedRowIndex, int selectedColumnIndex);
        void SelectionChanged(int selectedRowIndex, int selectedColumnIndex);
        void ScrollTopChanged(int scrollTop);

        GridCellType GetCellEditType(int rowIndex, int columnIndex);
        (string, object)[] GetCellEnumValues(int rowIndex, int columnIndex);
        object GetCellEditValue(int rowIndex, int columnIndex);
        void SetCellEditValue(int rowIndex, int columnIndex, object value);

        event EventHandler DataSourceChanged;
    }
}
