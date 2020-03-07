using System.Collections.Generic;

namespace AquaCalc5000.Parser
{
    public class CsvLine
    {
        public int LineNumber { get; set; }
        public string OriginalLine { get; set; }
        public List<string> Parts { get; set; } = new List<string>();
    }
}
