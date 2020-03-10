using System.Collections.Generic;

namespace AquaCalc5000.Parsers
{
    public class CsvLine
    {
        public int LineNumber { get; set; }
        public string OriginalLine { get; set; }
        public List<string> Parts { get; set; } = new List<string>();

        public CsvLine() { }

        public CsvLine(string originalLine, int lineNumber) : this()
        {
            OriginalLine = originalLine;
            LineNumber = lineNumber;
        }
    }
}
