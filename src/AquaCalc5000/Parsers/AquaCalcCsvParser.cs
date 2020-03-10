using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AquaCalc5000.Parsers
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

        public bool CanParse()
        {
            var firstLine = _csvParser.GetFirstNonEmptyLineOrNull();

            return firstLine != null &&
                   firstLine.OriginalLine.StartsWith(AquaCalcConstants.AquaCalc5000);
        }

        public ParsedData Parse()
        {
            var parsedData = new HeaderParser(_csvParser).Parse();

            parsedData.ObservationSectionLines = GetObservationSectionLines();
            parsedData.VerticalObservations = new ObservationSectionParser(parsedData.ObservationSectionLines).GetVerticals();

            var lastObservationLineNum = parsedData.ObservationSectionLines.Select(l => l.LineNumber).Max();
            parsedData.ErrorFlagLines = GetErrorFlagFooterLines(lastObservationLineNum);
            ParseErrorFlagsAsObservationComments(parsedData.VerticalObservations, parsedData.ErrorFlagLines);

            SanityCheckParsedData(parsedData);

            return parsedData;
        }

        private List<CsvLine> GetObservationSectionLines()
        {
            return _csvParser.GetAllLinesByFilter(l => l.Parts.Count >= 14);
        }

        private List<CsvLine> GetErrorFlagFooterLines(int lastObsLineNum)
        {
            var startLineNum = lastObsLineNum + 1;
            var lastLineNum = _csvParser.GetLastLineNum();

            var flagLines = _csvParser.GetAllLinesByFilter(l =>
                    l.LineNumber >= startLineNum && l.LineNumber <= lastLineNum)
                .Where(l => !string.IsNullOrWhiteSpace(l.OriginalLine))
                .OrderBy(l => l.LineNumber)
                .ToList();

            return flagLines;
        }

        private void SanityCheckParsedData(ParsedData parsedData)
        {
            var totalParsedPoints = parsedData.ObservationSectionLines.Count -1;
            if (parsedData.TotalStations != totalParsedPoints)
            {
                throw new ArgumentException($"The number of parsed observation points {totalParsedPoints} is not the same as " +
                                            $"the header Total Stations {parsedData.TotalStations}");
            }

            var totalParsedVerticals = parsedData.VerticalObservations.Count;
            if (parsedData.TotalVerticals != totalParsedVerticals)
            {
                throw new ArgumentException($"The number of parsed verticals {totalParsedVerticals} is not the same as " +
                                            $"the header Total Stations {parsedData.TotalVerticals}");
            }
        }

        private void ParseErrorFlagsAsObservationComments(List<VerticalObservation> verticalObservations,
            List<CsvLine> errorFlagLines)
        {
            var parser = new ErrorFlagParser(errorFlagLines);

            foreach (var observation in verticalObservations)
            {
                observation.Comments = parser.GetNoteByFlags(observation.Flags);
            }
        }
    }
}
