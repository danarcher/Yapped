using System;
using System.Drawing;
using System.Windows.Forms;

namespace Yapped.Grids.Generic
{
    /// <summary>
    /// A reusable data grid.
    /// </summary>
    public class Grid : UserControl
    {
        private VScrollBar scrollBar;
        private ToolTip toolTip;
        private Timer toolTipTimer, toolTipCancelTimer;
        private bool lastRowPartiallyVisible;
        private IGridHost host;
        private int selectedRowIndex;
        private int selectedColumnIndex;

        private CacheByColor<Brush> brushes = new CacheByColor<Brush>(color => new SolidBrush(color));
        private CacheByColor<Pen> pens = new CacheByColor<Pen>(color => new Pen(color));
        private Font boldFont;
        private StringFormat leftAlignedFormat;

        public Grid()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            scrollBar = new VScrollBar();
            Controls.Add(scrollBar);
            scrollBar.Dock = DockStyle.Right;
            scrollBar.ValueChanged += OnScrollBarValueChanged;

            toolTip = new ToolTip();

            BorderColor = SystemColors.ControlDark;
            SelectedRowBackColor = SystemColors.Control;
            SelectedRowForeColor = SystemColors.ControlText;
            SelectedCellBackColor = SystemColors.Highlight;
            SelectedCellForeColor = SystemColors.HighlightText;

            leftAlignedFormat = new StringFormat(StringFormat.GenericDefault);
            leftAlignedFormat.Alignment = StringAlignment.Near;
            leftAlignedFormat.LineAlignment = StringAlignment.Center;
            leftAlignedFormat.Trimming = StringTrimming.EllipsisCharacter;
            leftAlignedFormat.FormatFlags |= StringFormatFlags.NoWrap;

            ClearSelection();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                brushes.Dispose();
                pens.Dispose();
                boldFont?.Dispose();
                toolTipTimer?.Dispose();
                toolTipCancelTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        public IGridHost Host
        {
            get => host;
            set
            {
                if (host != value)
                {
                    if (host != null)
                    {
                        host.DataSourceChanged -= OnHostDataSourceChanged;
                    }
                    host = value;
                    if (host != null)
                    {
                        host.DataSourceChanged += OnHostDataSourceChanged;
                    }

                    // The host may already have its data source set.
                    OnHostDataSourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public int SelectedRowIndex
        {
            get => selectedRowIndex;
            set
            {
                if (host == null || host.RowCount < 1)
                {
                    if (selectedRowIndex != -1)
                    {
                        RaiseSelectionChanging();
                        selectedRowIndex = -1;
                        RaiseSelectionChanged();
                    }
                    return;
                }
                value = Math.Max(0, Math.Min(host.RowCount - 1, value));
                if (selectedRowIndex != value)
                {
                    RaiseSelectionChanging();
                    selectedRowIndex = value;
                    RaiseSelectionChanged();
                }
            }
        }

        public int SelectedColumnIndex
        {
            get => selectedColumnIndex;
            set
            {
                if (host == null || host.ColumnCount < 1)
                {
                    if (selectedColumnIndex != -1)
                    {
                        RaiseSelectionChanging();
                        selectedColumnIndex = -1;
                        RaiseSelectionChanged();
                    }
                    return;
                }
                value = Math.Max(0, Math.Min(host.ColumnCount - 1, value));
                if (selectedColumnIndex != value)
                {
                    RaiseSelectionChanging();
                    selectedColumnIndex = value;
                    RaiseSelectionChanged();
                }
            }
        }

        public int ScrollTop
        {
            get => scrollBar.Value;
            set
            {
                if (scrollBar.Value != value && value >= scrollBar.Minimum && value <= scrollBar.Maximum)
                {
                    scrollBar.Value = value;
                }
            }
        }

        public int RowCount => host?.RowCount ?? 0;

        public Color BorderColor { get; set; }
        public Color SelectedRowBackColor { get; set; }
        public Color SelectedRowForeColor { get; set; }
        public Color SelectedCellBackColor { get; set; }
        public Color SelectedCellForeColor { get; set; }

        private int RowHeight => Font.Height + 8;

        private bool TryGetFirstVisibleRowIndex(out int rowIndex)
        {
            if (host == null || host.RowCount <= 0)
            {
                rowIndex = -1;
                return false;
            }
            rowIndex = Math.Max(0, Math.Min(host.RowCount - 1, scrollBar.Value));
            return true;
        }

        private int GetVisibleRowCount(GridRowVisibility visibility)
        {
            var count = scrollBar.LargeChange;
            // Adjust if the caller requires fully visible rows.
            if (visibility == GridRowVisibility.Full && scrollBar.LargeChange > 1 && lastRowPartiallyVisible)
            {
                --count;
            }
            return count;
        }

        private bool TryGetLastVisibleRowIndex(GridRowVisibility visibility, out int rowIndex)
        {
            if (host == null || host.RowCount <= 0)
            {
                rowIndex = -1;
                return false;
            }
            rowIndex = Math.Max(0, Math.Min(host.RowCount - 1, scrollBar.Value + scrollBar.LargeChange));

            // Adjust if the caller requires fully visible rows.
            if (visibility == GridRowVisibility.Full && scrollBar.LargeChange > 1 && lastRowPartiallyVisible)
            {
                --rowIndex;
            }
            return true;
        }

        private bool TryGetRowBounds(int rowIndex, out Rectangle rect)
        {
            if (!TryGetFirstVisibleRowIndex(out int firstVisibleRowIndex) ||
                !TryGetLastVisibleRowIndex(GridRowVisibility.Any, out int lastVisibleRowIndex) ||
                rowIndex < firstVisibleRowIndex ||
                rowIndex > lastVisibleRowIndex)
            {
                rect = Rectangle.Empty;
                return false;
            }
            var x = 0;
            var y = RowHeight + (rowIndex - firstVisibleRowIndex) * RowHeight;
            var width = ClientSize.Width;
            var height = RowHeight;
            rect = new Rectangle(x, y, width, height);
            return true;
        }

        private bool TryGetCellBounds(int rowIndex, int columnIndex, out Rectangle rect)
        {
            if (!TryGetRowBounds(rowIndex, out Rectangle rowRect))
            {
                rect = Rectangle.Empty;
                return false;
            }

            var x = rowRect.X;
            var y = rowRect.Y;
            for (var i = 0; i < columnIndex; ++i)
            {
                x += host?.GetColumnWidth(this, i) ?? 0;
            }
            var width = host?.GetColumnWidth(this, columnIndex) ?? 0;
            var height = rowRect.Height;
            rect = new Rectangle(x, y, width, height);
            return true;
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateScrollBar();
            base.OnResize(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var delta = 8;
            if (e.Delta < 0)
            {
                scrollBar.Value = Math.Min(scrollBar.Value + delta, Math.Max(0, scrollBar.Maximum - scrollBar.LargeChange + 1));
            }
            else if (e.Delta > 0)
            {
                scrollBar.Value = Math.Max(scrollBar.Value - delta, 0);
            }
            CancelToolTip();
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            ShowOrUpdateToolTip();
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Don't cancel the tooltip immediately, since this event may be
            // triggered by the user mousing over the tooltip itself on their
            // way to another cell. Instead, if we don't reshow the tooltip
            // soon (on that other cell) then cancel it.
            CancelToolTipUnlessShowAgainSoon();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (HitTest(e.Location, out int rowIndex, out int columnIndex))
            {
                SelectedRowIndex = rowIndex;
                SelectedColumnIndex = columnIndex;
                ScrollToSelection(); // In case the user clicked a partially visible row.
            }
            base.OnMouseDown(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (HitTest(PointToClient(MousePosition), out int rowIndex, out int columnIndex))
            {
                SelectedRowIndex = rowIndex;
                SelectedColumnIndex = columnIndex;
                ScrollToSelection();
                TryEditCell();
            }
            base.OnDoubleClick(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (selectedColumnIndex > 0)
                    {
                        --SelectedColumnIndex;
                        ScrollToSelection();
                    }
                    break;
                case Keys.Right:
                    if (host != null && selectedColumnIndex < host.ColumnCount - 1)
                    {
                        ++SelectedColumnIndex;
                        ScrollToSelection();
                    }
                    break;
                case Keys.Up:
                    if (selectedRowIndex > 0)
                    {
                        --SelectedRowIndex;
                        ScrollToSelection();
                    }
                    break;
                case Keys.Down:
                    if (host != null && selectedRowIndex < host.RowCount - 1)
                    {
                        ++SelectedRowIndex;
                        ScrollToSelection();
                    }
                    break;
                case Keys.PageUp:
                    if (TryGetFirstVisibleRowIndex(out int firstVisibleRowIndex))
                    {
                        if (selectedRowIndex > firstVisibleRowIndex)
                        {
                            SelectedRowIndex = firstVisibleRowIndex;
                        }
                        else
                        {
                            SelectedRowIndex = Math.Max(0, selectedRowIndex - Math.Max(1, GetVisibleRowCount(GridRowVisibility.Any) - 1));
                            ScrollToSelection();
                        }
                    }
                    break;
                case Keys.PageDown:
                    if (TryGetLastVisibleRowIndex(GridRowVisibility.Full, out int lastVisibleRowIndex))
                    {
                        if (selectedRowIndex < lastVisibleRowIndex)
                        {
                            SelectedRowIndex = lastVisibleRowIndex;
                        }
                        else
                        {
                            SelectedRowIndex = Math.Min(host.RowCount - 1, selectedRowIndex + Math.Max(1, GetVisibleRowCount(GridRowVisibility.Any) - 1));
                            ScrollToSelection();
                        }
                    }
                    break;
                case Keys.Home:
                    if (e.Control && host != null && host.RowCount > 0)
                    {
                        SelectedRowIndex = 0;
                        ScrollToSelection();
                    }
                    break;
                case Keys.End:
                    if (e.Control && host != null && host.RowCount > 0)
                    {
                        SelectedRowIndex = host.RowCount - 1;
                        ScrollToSelection();
                    }
                    break;
                case Keys.F2:
                case Keys.Enter:
                    TryEditCell();
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            e.Graphics.Clear(SystemColors.Window);
            if (host == null)
            {
                // We've nothing to draw.
                return;
            }

            var defaultFont = Font;
            var rowHeight = RowHeight;

            // Draw column headers.
            var x = 0;
            for (var columnIndex = 0; columnIndex < host.ColumnCount; ++columnIndex)
            {
                var text = host.GetColumnName(columnIndex);
                var columnWidth = host.GetColumnWidth(this, columnIndex);
                var rect = new Rectangle(x, 0, columnWidth, rowHeight);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    graphics.FillRectangle(SystemBrushes.Control, rect);
                    graphics.DrawRectangle(SystemPens.ControlDark, rect);
                    rect.X += 4; rect.Width -= 4;
                    graphics.DrawString(text, defaultFont, SystemBrushes.ControlText, rect, leftAlignedFormat);
                }
                x += columnWidth;
            }

            // Draw rows.
            var style = new GridCellStyle();
            if (TryGetFirstVisibleRowIndex(out int firstVisibleRowIndex) &&
                TryGetLastVisibleRowIndex(GridRowVisibility.Any, out int lastVisibleRowIndex))
            {
                for (var rowIndex = firstVisibleRowIndex; rowIndex <= lastVisibleRowIndex; ++rowIndex)
                {
                    for (var columnIndex = 0; columnIndex < host.ColumnCount; ++columnIndex)
                    {
                        if (TryGetCellBounds(rowIndex, columnIndex, out Rectangle rect) &&
                            rect.Width > 0 && rect.Height > 0)
                        {
                            var text = host.GetCellDisplayValue(rowIndex, columnIndex);

                            // Set the default style for this cell.
                            style.SelectedRow = rowIndex == selectedRowIndex;
                            style.SelectedColumn = columnIndex == selectedColumnIndex;
                            style.SelectedCell = style.SelectedRow && style.SelectedColumn;
                            style.BackColor = SystemColors.Window;
                            style.ForeColor = SystemColors.WindowText;
                            style.BorderColor = SystemColors.ControlDark;
                            if (style.SelectedCell)
                            {
                                style.BackColor = SelectedCellBackColor;
                                style.ForeColor = SelectedCellForeColor;
                            }
                            else if (style.SelectedRow)
                            {
                                style.BackColor = SelectedRowBackColor;
                                style.ForeColor = SelectedRowForeColor;
                            }
                            style.BackBrush = null;
                            style.ForeBrush = null;
                            style.BorderPen = null;
                            style.Bold = false;

                            // Allow the host to restyle the row.
                            host.ModifyCellStyle(rowIndex, columnIndex, style);

                            // Create/fetch brushes, if the host didn't provide them.
                            if (style.BackBrush == null)
                            {
                                style.BackBrush = brushes.Get(style.BackColor);
                            }
                            if (style.ForeBrush == null)
                            {
                                style.ForeBrush = brushes.Get(style.ForeColor);
                            }
                            if (style.BorderPen == null)
                            {
                                style.BorderPen = pens.Get(style.BorderColor);
                            }

                            // Update the bold font if necessary.
                            var font = defaultFont;
                            if (style.Bold)
                            {
                                if (boldFont == null || boldFont.FontFamily.Name != defaultFont.FontFamily.Name || boldFont.Size != defaultFont.Size)
                                {
                                    boldFont?.Dispose();
                                    boldFont = new Font(defaultFont, FontStyle.Bold);
                                }
                                font = boldFont;
                            }

                            // Draw the cell.
                            graphics.FillRectangle(style.BackBrush, rect);
                            graphics.DrawRectangle(style.BorderPen, rect);
                            rect.X += 4; rect.Width -= 4;
                            graphics.DrawString(text, font, style.ForeBrush, rect, leftAlignedFormat);
                        }
                    }
                }
            }
        }

        private void OnHostDataSourceChanged(object sender, EventArgs e)
        {
            scrollBar.ValueChanged -= OnScrollBarValueChanged;
            scrollBar.Value = 0;
            scrollBar.ValueChanged += OnScrollBarValueChanged;
            UpdateScrollBar();
            Invalidate();
            ClearSelection();
        }

        private void OnScrollBarValueChanged(object sender, EventArgs e)
        {
            host?.ScrollTopChanged(scrollBar.Value);
            Invalidate();
        }

        private void UpdateScrollBar()
        {
            scrollBar.Minimum = 0;
            scrollBar.SmallChange = 1;
            var visibleRowCount = (ClientSize.Height - RowHeight) / RowHeight; // Subtract column header height, then allow space for rows.
            lastRowPartiallyVisible = ((ClientSize.Height - RowHeight) > visibleRowCount * RowHeight);
            scrollBar.LargeChange = visibleRowCount;
            if (host?.RowCount > 0)
            {
                scrollBar.Maximum = host.RowCount - 1; // This is an inclusive maximum.
            }
            else
            {
                scrollBar.Maximum = 0;
            }
            scrollBar.Visible = scrollBar.Maximum >= scrollBar.LargeChange || scrollBar.Value > 0;
        }

        private void ClearSelection()
        {
            RaiseSelectionChanging(); // Forced.
            selectedRowIndex = -1;
            selectedColumnIndex = -1;
            RaiseSelectionChanged(); // Forced.
            Invalidate();
        }

        public void ScrollToSelection()
        {
            if (selectedRowIndex >= 0 &&
                TryGetFirstVisibleRowIndex(out int firstVisibleRowIndex) &&
                TryGetLastVisibleRowIndex(GridRowVisibility.Full, out int lastVisibleRowIndex))
            {
                if (selectedRowIndex < firstVisibleRowIndex)
                {
                    scrollBar.Value = selectedRowIndex;
                    Invalidate();
                }
                else if (selectedRowIndex > lastVisibleRowIndex)
                {
                    scrollBar.Value = Math.Max(0, selectedRowIndex - GetVisibleRowCount(GridRowVisibility.Full));
                    Invalidate();
                }
            }
        }

        private void RaiseSelectionChanging()
        {
            host?.SelectionChanging(selectedRowIndex, selectedColumnIndex);
        }

        private void RaiseSelectionChanged()
        {
            CancelToolTip();
            Invalidate();
            host?.SelectionChanged(selectedRowIndex, selectedColumnIndex);
        }

        private bool HitTest(Point p, out int rowIndex, out int columnIndex)
        {
            if (TryGetFirstVisibleRowIndex(out int firstVisibleRowIndex) &&
                TryGetLastVisibleRowIndex(GridRowVisibility.Any, out int lastVisibleRowIndex))
            {
                for (var testRowIndex = firstVisibleRowIndex; testRowIndex <= lastVisibleRowIndex; ++testRowIndex)
                {
                    for (var testColumnIndex = 0; testColumnIndex < host.ColumnCount; ++testColumnIndex)
                    {
                        if (TryGetCellBounds(testRowIndex, testColumnIndex, out Rectangle rect))
                        {
                            if (rect.Contains(p))
                            {
                                rowIndex = testRowIndex;
                                columnIndex = testColumnIndex;
                                return true;
                            }
                        }
                    }
                }
            }
            rowIndex = -1;
            columnIndex = -1;
            return false;
        }

        private void ShowOrUpdateToolTip()
        {
            // Don't show a tooltip if the mouse isn't over a cell,
            // or if that cell's tooltip is already showing.
            var location = PointToClient(MousePosition);
            if (host == null ||
                !HitTest(location, out int rowIndex, out int columnIndex) ||
                !TryGetCellBounds(rowIndex, columnIndex, out Rectangle rect) ||
                (toolTip.Active && object.Equals(toolTip.Tag, (rowIndex, columnIndex))))
            {
                return;
            }

            // Show no tooltip if there is no text to show.
            var text = host.GetCellToolTip(rowIndex, columnIndex);
            if (string.IsNullOrEmpty(text))
            {
                CancelToolTip();
                return;
            }

            // Cancel any other tooltip related activity.
            toolTipCancelTimer?.Dispose();
            toolTipTimer?.Dispose();

            // Set up how we'll show the tooltip.
            Action showTip = () =>
            {
                // We'll show the tooltip under the cell.
                toolTip.Show(text, this, rect.Left, rect.Bottom - 1);
                // Tag the tooltip as belonging to this cell.
                toolTip.Tag = (rowIndex, columnIndex);
                // If we showed the tooltip on a timer, stop it now.
                toolTipTimer?.Stop();
            };

            if (toolTip.Tag != null)
            {
                // If the user is already interested in tooltips,
                // show the tooltip immediately.
                showTip();
                return;
            }

            // Show the tooltip soon if the user's interest doesn't wander.
            toolTipTimer = new Timer();
            toolTipTimer.Tick += (s, v) => showTip();
            toolTipTimer.Interval = 500;
            toolTipTimer.Start();
        }

        private void CancelToolTip()
        {
            toolTip.Tag = null;
            toolTip.Hide(this);
            toolTipTimer?.Dispose();
        }

        private void CancelToolTipUnlessShowAgainSoon()
        {
            toolTipCancelTimer?.Dispose();
            toolTipCancelTimer = new Timer();
            toolTipCancelTimer.Tick += (s, v) => CancelToolTip();
            toolTipCancelTimer.Interval = 500;
            toolTipCancelTimer.Start();
        }

        private bool TryEditCell()
        {
            if (host == null || selectedRowIndex == -1 || selectedColumnIndex == -1 ||
                host.GetCellEditType(selectedRowIndex, selectedColumnIndex) == GridCellType.None)
            {
                return false;
            }

            // Just toggle boolean values.
            if (host.GetCellEditType(selectedRowIndex, selectedColumnIndex) == GridCellType.Boolean)
            {
                host.SetCellEditValue(selectedRowIndex, selectedColumnIndex, !(bool)host.GetCellEditValue(selectedRowIndex, selectedColumnIndex));
                Invalidate();
                return true;
            }

            // Edit other values with a dialog.
            using (var dialog = new FormCellEdit())
            {
                dialog.DataType = host.GetCellEditType(selectedRowIndex, selectedColumnIndex);
                dialog.EnumValues = host.GetCellEnumValues(selectedRowIndex, selectedColumnIndex);
                dialog.Value = host.GetCellEditValue(selectedRowIndex, selectedColumnIndex);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    host.SetCellEditValue(selectedRowIndex, selectedColumnIndex, dialog.Value);
                    Invalidate();
                    return true;
                }
            }
            return false;
        }
    }
}
