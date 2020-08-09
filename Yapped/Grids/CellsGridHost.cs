using System;
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
        private readonly History history;
        private readonly GridSet grids;
        private ParamLayoutExtra extras;

        public CellsGridHost(History history, GridSet grids)
        {
            this.history = history;
            this.grids = grids;
        }

        public bool Initialized { get; set; }
        public override int ColumnCount => 3;
        public override int RowCount => DataSource?.Length ?? 0;

        public override void Initialize(Grid grid)
        {
            grid.SelectedRowIndex = history.Current[history.Current.Params.Selected].Cells.Selected;
            grid.SelectedColumnIndex = 1;
            grid.ScrollToSelection();
            extras = grids.Params.SelectedRowIndex >= 0 ? grids.ParamsHost.DataSource[grids.Params.SelectedRowIndex].Extra : null;
            Initialized = true;
        }

        public override string GetCellDisplayValue(int rowIndex, int columnIndex)
        {
            var item = DataSource[rowIndex];
            string text;
            switch (columnIndex)
            {
                case 0:
                    return item.Type.ToString();
                case 1:
                    text = item.Name?.ToString() ?? string.Empty;
                    var extra = GetExtra(rowIndex);
                    if (!string.IsNullOrWhiteSpace(extra?.DisplayName))
                    {
                        text = extra.DisplayName;
                    }
                    return text;
                case 2:
                    switch (item.Type)
                    {
                        case PARAM.CellType.x8:
                            text = $"0x{item.Value:X2}";
                            break;
                        case PARAM.CellType.x16:
                            text = $"0x{item.Value:X4}";
                            break;
                        case PARAM.CellType.x32:
                            text = $"0x{item.Value:X8}";
                            break;
                        default:
                            text = DataSource[rowIndex].Value?.ToString() ?? string.Empty;
                            break;
                    }
                    if (InspectLink(rowIndex, out LinkInfo linkInfo) == LinkStatus.Valid)
                    {
                        text += $" {linkInfo.TargetParam.Name} {linkInfo.TargetRow.Name}";
                    }
                    return text;
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
                    return grid.FontHeight * 2;
                case 1:
                    return grid.FontHeight * 16;
                case 2:
                    return grid.ClientSize.Width - grid.FontHeight * 18;
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
            
            // Style links.
            base.ModifyCellStyle(rowIndex, columnIndex, style);
            
            // Highlight invalid links.
            if (columnIndex == 2 && InspectLink(rowIndex) == LinkStatus.Invalid)
            {
                style.BackColor = Color.Pink;
                style.ForeColor = Color.Black;
                style.Underline = false;
            }
        }

        public override void SelectionChanged(int selectedRowIndex, int selectedColumnIndex)
        {
            if (Initialized)
            {
                history.Current[history.Current.Params.Selected].Cells.Selected = selectedRowIndex;
            }
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

        public override bool IsLinkClickable(int rowIndex, int columnIndex)
        {
            return columnIndex == 2 && InspectLink(rowIndex) == LinkStatus.Valid;
        }

        public override void LinkClicked(int rowIndex, int columnIndex)
        {
            if (InspectLink(rowIndex, out LinkInfo linkInfo) != LinkStatus.Valid)
            {
                return;
            }

            history.Push();
            grids.Params.SelectedRowIndex = linkInfo.TargetParamIndex;
            grids.Params.ScrollToSelection();
            grids.Rows.SelectedRowIndex = linkInfo.TargetRowIndex;
            grids.Rows.ScrollToSelection();
        }

        private ParamLayoutExtra.Entry GetExtra(int rowIndex)
        {
            extras.Entries.TryGetValue(DataSource[rowIndex].Name, out ParamLayoutExtra.Entry entry);
            return entry;
        }

        private LinkStatus InspectLink(int rowIndex) => InspectLink(rowIndex, out _);

        private LinkStatus InspectLink(int rowIndex, out LinkInfo linkInfo)
        {
            switch (DataSource[rowIndex].Type)
            {
                case PARAM.CellType.dummy8:
                case PARAM.CellType.fixstr:
                case PARAM.CellType.fixstrW:
                    // These data types are never indices into a param's rows.
                    linkInfo = null;
                    return LinkStatus.None;
            }

            var extra = GetExtra(rowIndex);
            if (extra == null)
            {
                // We have no extra information for this param, hence no links.
                linkInfo = null;
                return LinkStatus.None;
            }

            if (extra.Links.Count == 0)
            {
                // We have no links.
                linkInfo = null;
                return LinkStatus.None;
            }

            var value = Convert.ToInt64(DataSource[rowIndex].Value);
            if (value < 0)
            {
                // A negative value, which is often the default, can't be a valid index into a param's rows.
                linkInfo = null;
                if (value != -1)
                {
                    // These should really be set to -1.
                    return LinkStatus.Invalid;
                }
                return LinkStatus.None;
            }

            var row = grids.RowsHost.DataSource.Rows[grids.Rows.SelectedRowIndex];
            if (!extra.TryGetValidLink(row, out ParamLayoutExtra.Link link))
            {
                // We have no links under current conditions.
                linkInfo = null;
                return LinkStatus.None;
            }

            var targetParamIndex = grids.ParamsHost.DataSource.FindIndex(x => string.Equals(x.Name, link.Target));
            if (targetParamIndex < 0)
            {
                // We can't find the param the link wants to jump to.
                linkInfo = null;
                return LinkStatus.Invalid;
            }
            var param = grids.ParamsHost.DataSource[targetParamIndex];

            var targetRowIndex = param.Rows.FindIndex(x => (long)x.ID == value);
            if (targetRowIndex < 0)
            {
                // We can't find the row matching the current value.
                linkInfo = null;
                return LinkStatus.Invalid;
            }
            var targetRow = param.Rows[targetRowIndex];

            linkInfo = new LinkInfo
            {
                TargetParamIndex = targetParamIndex,
                TargetParam = param,
                TargetRowIndex = targetRowIndex,
                TargetRow = targetRow,
            };
            return LinkStatus.Valid;
        }

        private enum LinkStatus
        {
            None,
            Valid,
            Invalid,
        }

        private class LinkInfo
        {
            public int TargetParamIndex { get; set; }
            public ParamWrapper TargetParam { get; set; }

            public int TargetRowIndex { get; set; }
            public PARAM.Row TargetRow { get; set; }
        }
    }
}
