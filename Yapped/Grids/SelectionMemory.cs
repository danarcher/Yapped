using System;
using System.Collections.Generic;

namespace Yapped.Grids
{
    /// <summary>
    /// Helper class to remember selected (and top) rows and cells when viewing different params.
    /// </summary>
    internal class SelectionMemory
    {
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
            //foreach (Match match in Regex.Matches(settings.DGVIndices, @"[^,]+"))
            //{
            //    string[] components = match.Value.Split(':');
            //    dgvIndices[components[0]] = (int.Parse(components[1]), int.Parse(components[2]));
            //}
        }

        public string Save()
        {
            //var components = new List<string>();
            //foreach (string key in dgvIndices.Keys)
            //    components.Add($"{key}:{dgvIndices[key].Row}:{dgvIndices[key].Cell}");
            //settings.DGVIndices = string.Join(",", components);
            return string.Empty;
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
