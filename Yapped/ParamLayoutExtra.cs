using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SoulsFormats;

namespace Yapped
{
    internal class ParamLayoutExtra
    {
        public Dictionary<int, Entry> EntriesByIndex { get; } = new Dictionary<int, Entry>();

        public static Dictionary<string, ParamLayoutExtra> Load(string path, Dictionary<string, PARAM.Layout> layouts)
        {
            var extras = new Dictionary<string, ParamLayoutExtra>();

            List<string> lines;
            try
            {
                lines = File.ReadAllLines(path)
                            .Select(x => x.Trim())
                            .Where(x => x.Length > 0)
                            .ToList();
            }
            catch (Exception)
            {
                return extras;
            }

            PARAM.Layout layout = null;
            ParamLayoutExtra extra = null;
            lines.ForEach(line => LoadLine(line, layouts, ref layout, extras, ref extra));
            return extras;
        }

        private static void LoadLine(string line, Dictionary<string, PARAM.Layout> layouts, ref PARAM.Layout layout, Dictionary<string, ParamLayoutExtra> layoutExtras, ref ParamLayoutExtra layoutExtra)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string part(int x) => parts.Length > x ? parts[x] : "?";

            var key = part(0);
            if (key.EndsWith(":"))
            {
                // Switch layouts (params).
                key = key.TrimEnd(':');
                if (!layouts.TryGetValue(key, out layout))
                {
                    throw new InvalidDataException($"There is no layout param named {key}.");
                }
                if (!layoutExtras.TryGetValue(key, out layoutExtra))
                {
                    layoutExtra = new ParamLayoutExtra();
                    layoutExtras.Add(key, layoutExtra);
                }
                return;
            }

            if (layoutExtra == null)
            {
                // No current layout.
                return;
            }

            int? cellIndex = null;
            if (key.StartsWith("#") && int.TryParse(key.Substring(1), NumberStyles.Any, CultureInfo.InvariantCulture, out int keyInteger))
            {
                cellIndex = keyInteger;
            }

            // From the param layout, try to find cell names matching this
            // key, and add extra information for each.
            var keyRegex = cellIndex == null ? new Regex(key) : null;
            for (var index = 0; index < layout.Count; ++index)
            {
                var cell = layout[index];
                var match = keyRegex?.Match(cell.Name);
                if (match != null && match.Success || cellIndex == index)
                {
                    if (!layoutExtra.EntriesByIndex.TryGetValue(index, out Entry entry))
                    {
                        entry = new Entry();
                        layoutExtra.EntriesByIndex.Add(index, entry);
                    }

                    var type = part(1);
                    if (string.Equals(type, "alias", StringComparison.OrdinalIgnoreCase))
                    {
                        // Add an alias to the current entry.
                        entry.DisplayName = string.Join(" ", parts.Skip(2));
                    }
                    else if (string.Equals(type, "link", StringComparison.OrdinalIgnoreCase))
                    {
                        var link = new Link();
                        link.Target = part(2);
                        if (string.Equals(part(3), "if", StringComparison.OrdinalIgnoreCase))
                        {
                            var condition = new Condition();
                            condition.Key = part(4);
                            if (match.Groups.Count > 1)
                            {
                                condition.Key = condition.Key.Replace("$1", match.Groups[1].Captures[0].Value);
                            }
                            switch (part(5))
                            {
                                case "==":
                                    condition.Operator = Operator.Equals;
                                    break;
                                case "!=":
                                    condition.Operator = Operator.NotEquals;
                                    break;
                                case ">":
                                    condition.Operator = Operator.GreaterThan;
                                    break;
                                case "<":
                                    condition.Operator = Operator.LessThan;
                                    break;
                            }
                            int.TryParse(part(6), NumberStyles.Any, CultureInfo.InvariantCulture, out int value);
                            condition.Value = value;
                            link.Conditions.Add(condition);
                        }
                        entry.Links.Add(link);
                    }
                }
            }
        }

        public class Entry
        {
            public string DisplayName { get; set; }
            public List<Link> Links { get; set; } = new List<Link>();

            public bool TryGetValidLink(PARAM.Row row, out Link link)
            {
                link = Links.FirstOrDefault(x => x.IsValid(row));
                return link != null;
            }
        }

        public class Link
        {
            public List<Condition> Conditions { get; } = new List<Condition>();
            public string Target { get; set; }

            public bool IsValid(PARAM.Row row) => Conditions.TrueForAll(x => x.Evaluate(row));
        }

        public class Condition
        {
            public string Key { get; set; }
            public Operator Operator { get; set; }
            public long Value { get; set; }

            public bool Evaluate(PARAM.Row row)
            {
                var cell = row[Key];
                if (cell == null)
                {
                    return false;
                }

                var value = Convert.ToInt64(cell.Value);
                switch (Operator)
                {
                    case Operator.Equals:
                        return value == Value;
                    case Operator.NotEquals:
                        return value != Value;
                    case Operator.LessThan:
                        return value < Value;
                    case Operator.GreaterThan:
                        return value > Value;
                    default:
                        return false;
                }
            }
        }

        public enum Operator
        {
            Equals = 0,
            NotEquals = 1,
            LessThan = 2,
            GreaterThan = 3,
        }
    }
}
