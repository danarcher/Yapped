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
            var item = DataSource[rowIndex];
            switch (columnIndex)
            {
                case 0:
                    return item.Type.ToString();
                case 1:
                    return item.Name?.ToString() ?? string.Empty;
                case 2:
                    switch (item.Type)
                    {
                        case PARAM.CellType.x8:
                            return $"0x{item.Value:X2}";
                        case PARAM.CellType.x16:
                            return $"0x{item.Value:X4}";
                        case PARAM.CellType.x32:
                            return $"0x{item.Value:X8}";
                        default:
                            return DataSource[rowIndex].Value?.ToString() ?? string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        public override string GetCellToolTip(int rowIndex, int columnIndex)
        {
            var text = DataSource[rowIndex].Description;
            if (string.IsNullOrEmpty(text))
            {
                text = "No description available.";
            }
            return text;
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

        public override GridCellType GetCellEditType(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    switch (DataSource[rowIndex].Type)
                    {
                        case PARAM.CellType.dummy8:
                            return GridCellType.None;
                        case PARAM.CellType.b8:
                        case PARAM.CellType.b16:
                        case PARAM.CellType.b32:
                            return GridCellType.Boolean;
                        case PARAM.CellType.u8:
                            return GridCellType.Byte;
                        case PARAM.CellType.x8:
                            return GridCellType.HexByte;
                        case PARAM.CellType.s8:
                            return GridCellType.SByte;
                        case PARAM.CellType.u16:
                            return GridCellType.UInt16;
                        case PARAM.CellType.x16:
                            return GridCellType.HexUInt16;
                        case PARAM.CellType.s16:
                            return GridCellType.Int16;
                        case PARAM.CellType.u32:
                            return GridCellType.UInt32;
                        case PARAM.CellType.x32:
                            return GridCellType.HexUInt32;
                        case PARAM.CellType.s32:
                            return GridCellType.Int32;
                        case PARAM.CellType.f32:
                            return GridCellType.Single;
                        case PARAM.CellType.fixstr:
                            return GridCellType.String;
                        case PARAM.CellType.fixstrW:
                            return GridCellType.String;
                    }
                    break;
                default:
                    return GridCellType.None;
            }
            return base.GetCellEditType(rowIndex, columnIndex);
        }

        public override object GetCellEditValue(int rowIndex, int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return DataSource[rowIndex].Value;
                default:
                    return null;
            }
        }

        public override void SetCellEditValue(int rowIndex, int columnIndex, object value)
        {
            switch (columnIndex)
            {
                case 2:
                    DataSource[rowIndex].Value = value;
                    break;
                default:
                    break;
            }
        }
    }
}
