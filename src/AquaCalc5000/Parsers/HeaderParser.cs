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
                GageId = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.GageId),
                StartDate = GetVisitDate(),
                Transect = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.Transect),
                UserId = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.UserId),

                StaffStageBegin = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.ShBegin),
                StaffStageEnd = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.ShEnd),
                LoggerStageBegin = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.GhBegin),
                LoggerStageEnd = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.GhEnd),

                MeterId = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.MeterId),
                SoundingWeight = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.SoundingWt),
                StartMode = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.StartMeasAt),
                MeterType = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.MeterType),
                MeterConst1 = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeterConstC1),
                MeterConst2 = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeterConstC2),
                MeterConst3 = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeterConstC3),
                MeterConst4 = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeterConstC4),
                MeterConst5 = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeterConstC5),

                UnitSystem = _csvParser.GetRequiredStringByLabel(AquaCalcConstants.MeasSystem),

                TotalVerticals = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.TotalVerticals),
                TotalStations = _csvParser.GetRequiredIntByLabel(AquaCalcConstants.TotalStations),
                TotalWidth = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.TotalWidth),
                TotalArea = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.TotalArea),
                TotalDischarge = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.TotalDischarge),

                MeanVelocity = _csvParser.GetRequiredDoubleByLabel(AquaCalcConstants.MeanVelocity),
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
