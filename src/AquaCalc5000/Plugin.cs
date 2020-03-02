using System;
using System.IO;
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
            return DummyResult(appender, null);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo targetLocation,
            IFieldDataResultsAppender appender, ILog logger)
        {
            return DummyResult(appender, targetLocation);
        }

        private ParseFileResult DummyResult(IFieldDataResultsAppender appender, LocationInfo targetLocation)
        {
            if (targetLocation == null)
            {
                targetLocation = appender.GetLocationByIdentifier("Foo");
            }

            appender.AddFieldVisit(targetLocation, new FieldVisitDetails(new DateTimeInterval(
                new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromHours(1))));

            return ParseFileResult.SuccessfullyParsedAndDataValid();
        }
    }
}
