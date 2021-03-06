﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Yapped.Grids
{
    internal class History
    {
        // History is not infinite, since it grows with mouse clicks.
        private static readonly int MaximumHistoryDepth = 16;

        private List<Entry> entries;
        private int index;

        public History() => Clear();

        public Entry Current { get => entries[index]; }

        /// <summary>
        /// Raised when the current history entry is changed.
        /// </summary>
        public event Action CurrentChanged;

        /// <summary>
        /// Raised when the past or future is changed, including when the current history entry is changed.
        /// </summary>
        public event Action TimelineChanged;

        public bool CanGoBack => index > 0;

        public bool CanGoForward => index < entries.Count - 1;

        public void GoBack()
        {
            index = Math.Max(0, index - 1);
            CurrentChanged?.Invoke();
            TimelineChanged?.Invoke();
        }

        public void GoForward(bool quiet = false)
        {
            index = Math.Min(entries.Count - 1, index + 1);
            if (!quiet)
            {
                CurrentChanged?.Invoke();
            }
            TimelineChanged?.Invoke();
        }

        public void DiscardForward()
        {
            if (CanGoForward)
            {
                entries.RemoveRange(index + 1, entries.Count - (index + 1));
            }
            TimelineChanged?.Invoke();
        }

        public void Push(bool quiet = false)
        {
            DiscardForward();
            entries.Add(Current.Clone());
            Clean();
            GoForward(quiet);
        }

        public void Clean()
        {
            entries.ForEach(x => x.Clean());
            while (entries.Count > MaximumHistoryDepth && index > 0)
            {
                entries.RemoveAt(0);
                --index;
            }
            TimelineChanged?.Invoke();
        }

        public string Save()
        {
            var serialize = new Serialize
            {
                Entries = entries,
                Index = index,
            };
            return JsonConvert.SerializeObject(serialize);
        }

        public void Load(string value)
        {
            try
            {
                var serialize = JsonConvert.DeserializeObject<Serialize>(value);
                entries = serialize.Entries;
                index = serialize.Index;
                CurrentChanged?.Invoke();
                TimelineChanged?.Invoke();
            }
            catch (Exception)
            {
                Clear();
            }
        }

        private void Clear()
        {
            entries = new List<Entry> { new Entry() };
            index = 0;
            CurrentChanged?.Invoke();
            TimelineChanged?.Invoke();
        }

        private class Serialize
        {
            public List<Entry> Entries { get; set; }
            public int Index { get; set; }
        }

        public class Entry
        {
            public Position Params { get; set; } = new Position();
            public Dictionary<int, ParamEntry> Map = new Dictionary<int, ParamEntry>();

            public ParamEntry this[int paramIndex]
            {
                get
                {
                    if (!Map.TryGetValue(paramIndex, out ParamEntry entry))
                    {
                        entry = new ParamEntry();
                        Map.Add(paramIndex, entry);
                    }
                    return entry;
                }
            }

            public Entry Clone()
            {
                var clone = new Entry();
                clone.Params = Params.Clone();
                foreach (var pair in Map)
                {
                    clone.Map.Add(pair.Key, pair.Value.Clone());
                }
                return clone;
            }

            public void Clean()
            {
                var emptyEntries = Map.Where(x => x.Value.IsEmpty).ToArray();
                foreach (var pair in emptyEntries)
                {
                    Map.Remove(pair.Key);
                }
            }
        }

        public class ParamEntry
        {
            public Position Rows { get; set; } = new Position();
            public Position Cells { get; set; } = new Position();

            [JsonIgnore]
            public bool IsEmpty => Rows.IsEmpty && Cells.IsEmpty;

            public ParamEntry Clone()
            {
                return new ParamEntry
                {
                    Rows = Rows.Clone(),
                    Cells = Cells.Clone(),
                };
            }
        }

        public class Position
        {
            public int Selected { get; set; }

            [JsonIgnore]
            public bool IsEmpty => Selected <= 0;

            public Position Clone()
            {
                return new Position
                {
                    Selected = Selected,
                };
            }
        }
    }
}
