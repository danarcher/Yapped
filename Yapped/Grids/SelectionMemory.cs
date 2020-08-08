using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Yapped.Grids
{
    /// <summary>
    /// Helper class to remember selected (and top) rows and cells when viewing different params.
    /// </summary>
    internal class SelectionMemory
    {
        public int SelectedParam { get; set; }
        public int TopParam { get; set; }
        public Instance SelectedRow { get; } = new Instance();
        public Instance SelectedCell { get; } = new Instance();
        public Instance TopRow { get; } = new Instance();
        public Instance TopCell { get; } = new Instance();

        /// <summary>
        /// Greater than zero if storing values should have any effect.
        /// </summary>
        public int StoreEnabled
        {
            get { return SelectedRow.StoreEnabled; }
            set
            {
                SelectedRow.StoreEnabled = value;
                SelectedCell.StoreEnabled = value;
                TopRow.StoreEnabled = value;
                TopCell.StoreEnabled = value;
            }
        }

        /// <summary>
        /// Store the name of the currently selected param.
        /// </summary>
        public void StoreParamName(string name)
        {
            SelectedRow.StoreParamName(name);
            SelectedCell.StoreParamName(name);
            TopRow.StoreParamName(name);
            TopCell.StoreParamName(name);
        }

        public void Load(string saved)
        {
            var parts = saved.Split(';');
            if (parts.Length >= 6)
            {
                if (int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int selectedParam))
                {
                    SelectedParam = selectedParam;
                }
                if (int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int topParam))
                {
                    TopParam = topParam;
                }
                SelectedRow.Load(parts[2]);
                SelectedCell.Load(parts[3]);
                TopRow.Load(parts[4]);
                TopCell.Load(parts[5]);
            }
        }

        public string Save()
        {
            return string.Join(";",
                SelectedParam.ToString(CultureInfo.InvariantCulture),
                TopParam.ToString(CultureInfo.InvariantCulture),
                SelectedRow.Save(),
                SelectedCell.Save(),
                TopRow.Save(),
                TopCell.Save());
        }

        public class Instance
        {
            private Dictionary<string, int> map = new Dictionary<string, int>();
            private string name;

            /// <summary>
            /// Greater than zero if storing values should have any effect.
            /// </summary>
            public int StoreEnabled { get; set; } = 1;

            public void StoreParamName(string name) => this.name = name;

            public void StoreValue(int value)
            {
                if (StoreEnabled > 0 && !string.IsNullOrEmpty(name))
                {
                    map[name] = value;
                }
            }

            public bool RecallValue(out int value)
            {
                if (string.IsNullOrEmpty(name) || !map.TryGetValue(name, out value))
                {
                    value = -1;
                    return false;
                }
                return true;
            }

            public void Load(string saved)
            {
                map.Clear();
                var pairs = saved.Split(',');
                foreach (var pair in pairs)
                {
                    var parts = pair.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int value))
                    {
                        map.Add(parts[0], value);
                    }
                }

            }

            public string Save()
            {
                return string.Join(",", map.Keys.Select(x => $"{x}={map[x].ToString(CultureInfo.InvariantCulture)}"));
            }
        }

        /// <summary>
        /// Temporary stop storing values. For use within a using block.
        /// </summary>
        public IDisposable WithoutStoringValues() => new StoreDisableHelper(this);

        /// <summary>
        /// Helper class to temporarily stop storing values.
        /// </summary>
        private class StoreDisableHelper : IDisposable
        {
            private readonly SelectionMemory memory;

            public StoreDisableHelper(SelectionMemory memory)
            {
                this.memory = memory;
                // Disable storing values.
                --memory.StoreEnabled;
            }

            public void Dispose()
            {
                // Re-enable storing values.
                ++memory.StoreEnabled;
            }
        }
    }
}
