using System.IO;
using AquaCalc5000.Mappers;
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
                targetLocation = appender.GetLocationByIdentifier(parsedData.GageId);
            }

            var visitDetails = new VisitMapper(parsedData).GetVisitDetails(targetLocation.UtcOffset);
            logger.Info($"Got visit details: '{visitDetails.StartDate:s}@{targetLocation.LocationIdentifier}'");

            var visitInfo = appender.AddFieldVisit(targetLocation,visitDetails);
            
            AppendActivity(appender, parsedData, visitInfo, logger);

            return ParseFileResult.SuccessfullyParsedAndDataValid();
        }

        private void AppendActivity(IFieldDataResultsAppender appender, ParsedData parsedData,
            FieldVisitInfo visitInfo, ILog logger)
        {
            var visitInterval = new DateTimeInterval(visitInfo.StartDate, visitInfo.EndDate);
            var dischargeActivity = new ActivityMapper(parsedData, visitInterval)
                .GetDischargeActivity();
            logger.Info($"Got discharge activity for {visitInfo.StartDate:s}@{visitInfo.LocationInfo.LocationIdentifier}.");

            appender.AddDischargeActivity(visitInfo, dischargeActivity);
        }
    }
}
