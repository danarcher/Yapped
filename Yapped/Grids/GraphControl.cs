using System;
using System.Drawing;
using System.Windows.Forms;
using Yapped.Grids.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// A simple graph control.
    /// </summary>
    internal class GraphControl : UserControl
    {
        private readonly CacheByColor<Pen> pens = new CacheByColor<Pen>(color => new Pen(color));
        private GraphData data;

        public GraphControl()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pens?.Dispose();
            }
            base.Dispose(disposing);
        }

        public GraphData Data
        {
            get => data;
            set
            {
                if (data != value)
                {
                    data = value;
                    Invalidate();
                }
            }
        }

        public int Y { get; private set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            if (data != null)
            {
                if (data.Series1 != null)
                {
                    var xRange = data.XMaximum - data.XMinimum;
                    var yRange = data.YMaximum - data.YMinimum;
                    var width = ClientSize.Width;
                    var height = ClientSize.Height - 1;
                    
                    Pen pen;

                    // Draw grid.
                    pen = pens.Get(Color.FromArgb(230, 230, 230));
                    for (var i = 0; i < 10; ++i)
                    {
                        e.Graphics.DrawLine(pen, new PointF(0, i / 10.0f * height), new PointF(width, i / 10.0f * height));
                        e.Graphics.DrawLine(pen, new PointF(i / 10.0f * width, 0), new PointF(i / 10.0f * width, height));
                    }

                    // Draw reference line.
                    pen = pens.Get(Color.FromArgb(200, 200, 200));
                    e.Graphics.DrawLine(pen, new Point(0, height), new Point(width, 0));

                    // Draw series.
                    pen = pens.Get(data.Series1Color);
                    Point? previous = null;
                    foreach (var point in data.Series1)
                    {
                        var p = new Point
                        {
                            X = Math.Max(0, Math.Min(width, (int)Math.Round((point.X - data.XMinimum) / xRange * width))),
                            Y = Math.Max(0, Math.Min(height, height - (int)Math.Round((point.Y - data.YMinimum) / yRange * height)))
                        };
                        if (previous != null)
                        {
                            e.Graphics.DrawLine(pen, previous.Value, p);
                        }
                        previous = p;
                    }
                }
            }
            base.OnPaint(e);
        }
    }

    internal class GraphData
    {
        public float XMinimum { get; set; }
        public float XMaximum { get; set; }
        public float YMinimum { get; set; }
        public float YMaximum { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public PointF[] Series1 { get; set; }
        public Color Series1Color { get; set; }
    }
}
