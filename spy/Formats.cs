using System.Collections.Generic;

namespace spy.format
{
    public interface IFormat
    {
    }

    public class StringFormat : IFormat
    {
        public string Entry { get; set; }

        public static implicit operator string(StringFormat sf) => sf.Entry;

        public static implicit operator StringFormat(string s)
        {
            return new StringFormat { Entry = s };
        }
    }
    
    public class CsvRow : IFormat
    {
        public HashSet<string> Fields { get; set; }
        public Dictionary<string, string> Row { get; set; }
    }
}