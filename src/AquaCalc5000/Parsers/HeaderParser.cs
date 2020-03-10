using System;
using System.Globalization;

namespace AquaCalc5000.Parsers
{
    public class HeaderParser
    {
        private readonly CsvParser _csvParser;

        public HeaderParser(CsvParser csvParser)
        {
            _csvParser = csvParser;
        }

        public ParsedData Parse()
        {
            return new ParsedData
            {
                LocationIdentifier = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.GageId),
                StartDate = GetVisitDate(),
                Transect = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.Transect),
                TotalVerticals = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.TotalVerticals),
                TotalStations = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.TotalStations),
            };
        }

        public DateTime GetVisitDate()
        {
            var dateString = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.Date);

            if (!DateTime.TryParseExact(dateString, new[] { "M/d/yyyy", "M/d/yy" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                throw new ArgumentException($"Not a supported date string '{dateString}'");
            }

            if (parsedDate.Year <= 1980)
            {
                parsedDate = parsedDate.AddYears(100); //To address a known bug in AquaCalc 5000: year "20" is printed out as "1920".
            }

            return parsedDate;
        }
    }
}
