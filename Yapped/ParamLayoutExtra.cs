using System;
using System.Collections.Generic;
using System.Linq;
using SoulsFormats;

namespace Yapped
{
    internal class ParamLayoutExtra
    {
        public Dictionary<string, Entry> Entries { get; } = new Dictionary<string, Entry>();

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
