using System;
using System.Collections.Generic;
using System.Drawing;

namespace Yapped.Grids.Generic
{
    /// <summary>
    /// A cache of IDisposable objects, accessed by Color.
    /// Used for Brushes and Pens to minimise GDI+ overhead.
    /// </summary>
    public class CacheByColor<T> : IDisposable where T : IDisposable
    {
        private Func<Color, T> create;

        public CacheByColor(Func<Color, T> create)
        {
            this.create = create;
        }

        private Dictionary<Color, T> cache = new Dictionary<Color, T>();

        public T Get(Color color)
        {
            T value;
            if (!cache.TryGetValue(color, out value))
            {
                value = create(color);
                cache.Add(color, value);
            }
            return value;
        }

        public void Dispose()
        {
            foreach (var value in cache.Values)
            {
                value.Dispose();
            }
        }
    }
}
