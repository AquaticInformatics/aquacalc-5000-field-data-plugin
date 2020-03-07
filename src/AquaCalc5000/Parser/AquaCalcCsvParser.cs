using System;
using System.IO;
using System.Text;

namespace AquaCalc5000.Parser
{
    public class AquaCalcCsvParser
    {
        private readonly CsvParser _csvParser;

        public AquaCalcCsvParser(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Incoming file stream is null");
            }

            var csvText = GetAllTextFromStream(stream);
            _csvParser = new CsvParser(csvText);
        }

        public static string GetAllTextFromStream(Stream stream, bool leaveOpen = false)
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: leaveOpen))
            {
                return streamReader.ReadToEnd();
            }
        }

        public ParsedData Parse()
        {
            return new ParsedData
            {
                LocationIdentifier = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.GageId),
            };
        }

        public bool CanParse()
        {
            var firstLine = _csvParser.GetFirstNonEmptyLineOrNull();

            return firstLine != null && 
                   firstLine.OriginalLine.StartsWith(AquaCalcConstants.AquaCalc5000);
        }
    }
}
