using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    internal class GridSet
    {
        public Grid Params { get; set; }
        public Grid Rows { get; set; }
        public Grid Cells { get; set; }
        public ParamsGridHost ParamsHost { get; set; }
        public RowsGridHost RowsHost { get; set; }
        public CellsGridHost CellsHost { get; set; }
    }
}
