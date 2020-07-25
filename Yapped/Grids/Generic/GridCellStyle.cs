using System.Drawing;

namespace Yapped.Grids.Generic
{
    /// <summary>
    /// The style with which a grid cell will be rendered.
    /// </summary>
    public class GridCellStyle
    {
        public bool SelectedRow;
        public bool SelectedColumn;
        public bool SelectedCell;

        public Color BorderColor;
        public Color BackColor;
        public Color ForeColor;

        public Pen BorderPen;
        public Brush BackBrush;
        public Brush ForeBrush;

        public bool Bold;
    }
}
