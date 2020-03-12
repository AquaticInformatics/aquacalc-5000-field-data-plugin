using System;
using System.Linq;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework.DataModel;

namespace AquaCalc5000.Mappers
{
    public class VisitMapper
    {
        private readonly ParsedData _parsedData;
        public VisitMapper(ParsedData parsedData)
        {
            _parsedData = parsedData;
        }

        public FieldVisitDetails GetVisitDetails(TimeSpan locationUtcOffset)
        {
            var visitInterval = GetVisitInterval(locationUtcOffset);

            return new FieldVisitDetails(visitInterval)
            {
                Party = _parsedData.UserId
            };
        }

        private DateTimeInterval GetVisitInterval(TimeSpan locationUtcOffset)
        {
            var allObservationTimes = _parsedData.VerticalObservations
                .SelectMany(v => v.ObservationPoints)
                .Select(p => p.MeasuredAtTime)
                .ToList();

            var minTime = allObservationTimes.Min();
            var maxTime = allObservationTimes.Max();
            var start = new DateTimeOffset(_parsedData.StartDate.Add(minTime), locationUtcOffset);
            var duration = maxTime - minTime;

            return new DateTimeInterval(start, duration);
        }
    }
}
