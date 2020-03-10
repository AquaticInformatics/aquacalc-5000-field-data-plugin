using System;
using System.IO;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.Results;

namespace AquaCalc5000
{
    public class Plugin : IFieldDataPlugin
    {
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender appender, ILog logger)
        {
            return ParseFile(fileStream,null, appender, logger);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo targetLocation,
            IFieldDataResultsAppender appender, ILog logger)
        {
            var parser = new AquaCalcCsvParser(fileStream);
            if (!parser.CanParse())
            {
                return ParseFileResult.CannotParse();
            }

            var parsedData = parser.Parse();

            if (targetLocation == null)
            {
                targetLocation = appender.GetLocationByIdentifier(parsedData.LocationIdentifier);
            }

            appender.AddFieldVisit(targetLocation, new FieldVisitDetails(new DateTimeInterval(
                new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromHours(1))));

            return ParseFileResult.SuccessfullyParsedAndDataValid();
        }
    }
}
