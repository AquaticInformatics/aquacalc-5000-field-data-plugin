using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AquaCalc5000.Parser
{
    public class CsvParser
    {
        private const char DelimiterChar = ',';
        
        private readonly List<CsvLine> _csvLines;

        public CsvParser(string csvText)
        {
            _csvLines = ParseToCsvLines(csvText);
        }

        private List<CsvLine> ParseToCsvLines(string csvText)
        {
            var csvLines = new List<CsvLine>();

            if (csvText == null)
            {
                return csvLines;
            }

            using (var reader = new StringReader(csvText))
            {
                string line;
                var lineNumber = 1; 
                while ((line = reader.ReadLine()) != null)
                {
                    csvLines.Add(new CsvLine
                    {
                        LineNumber = lineNumber++,
                        OriginalLine = line,
                        Parts = SplitToParts(line)
                    });
                }
            }

            return csvLines;
        }

        private List<string> SplitToParts(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new List<string>();
            }

            return line.Split(DelimiterChar).Select(s => s.Trim()).ToList();
        }

        public CsvLine GetFirstNonEmptyLineOrNull()
        {
            return _csvLines.FirstOrDefault(l =>
                !string.IsNullOrWhiteSpace(l.OriginalLine));
        }

        public string GetRequiredStringByLabel(string label)
        {
            var foundLine = GetLineByLabelOrNull(label);

            if (foundLine == null)
            {
                throw new ArgumentException($"No line starting with '{label}' is found.", nameof(label));
            }

            var parts = foundLine.Parts;
            if (parts.Count < 2)
            {
                throw new ArgumentException($"Required value is not found for '{label}'");
            }

            var combinedString = string.Join(DelimiterChar.ToString(), 
                parts.Skip(1).Where(p => !string.IsNullOrWhiteSpace(p)));

            if (string.IsNullOrWhiteSpace(combinedString))
            {
                throw new ArgumentException($"Required value is empty for '{label}'");
            }

            return combinedString;
        }

        public CsvLine GetLineByLabelOrNull(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            return _csvLines.FirstOrDefault(l =>
                l.Parts.Any() &&
                string.Equals(l.Parts.First(), label, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
