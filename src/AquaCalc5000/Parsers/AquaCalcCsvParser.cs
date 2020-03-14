using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AquaCalc5000.Config;

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
            var firstLine = _csvParser.GetFirstLineByFilterOrNull(csvLine => 
                csvLine != null && 
                csvLine.OriginalLine.StartsWith(AquaCalcConstants.AquaCalc5000));

            return firstLine != null;
        }

        public ParsedData Parse()
        {
            var config = new ConfigLoader().Load();

            var parsedData = new HeaderParser(_csvParser).Parse();

            parsedData.GageId = ConvertGageIdToLocationIdentifier(parsedData.GageId, config);

            parsedData.ObservationSectionLines = GetObservationSectionLines();
            parsedData.VerticalObservations = new ObservationSectionParser(parsedData.ObservationSectionLines)
                .GetVerticalObservations();

            var lastObservationLineNum = parsedData.ObservationSectionLines.Select(l => l.LineNumber).Max();
            parsedData.ErrorFlagLines = GetErrorFlagFooterLines(lastObservationLineNum);
            ParseErrorFlagsAsObservationComments(parsedData.VerticalObservations, parsedData.ErrorFlagLines);

            return parsedData;
        }

        private string ConvertGageIdToLocationIdentifier(string gageId, Config.Config config)
        {
            if (!config.AssumeUsgsSiteIdentifiers)
                return gageId;

            return int.TryParse(gageId, out var numericId)
                ? $"{numericId:D8}"
                : gageId;
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

        private void ParseErrorFlagsAsObservationComments(List<VerticalObservation> verticalObservations,
            List<CsvLine> errorFlagLines)
        {
            var parser = new ErrorFlagParser(errorFlagLines);

            foreach (var observation in verticalObservations)
            {
                var flagComment = parser.GetNoteByFlags(observation.Flags);
                observation.Comments = string.IsNullOrWhiteSpace(observation.Comments)
                    ? flagComment
                    : $"{observation.Comments}{Environment.NewLine}{flagComment}";
            }
        }
    }
}
