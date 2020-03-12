using System;
using System.Linq;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;

namespace AquaCalc5000.Mappers
{
    public class CommonMapper
    {
        public const string WallMeasurementMappedToPoint6 = "Wall measurement. Mapped to .6"; 
        public const string BottomMeasurementMappedToPoint6 = "Bottom measurement. Mapped to .6";
        public const string CosVfEquals = "COS:VF=";

        public static DeploymentMethodType GetDeploymentMethodBySoundingWeight(double soundingWeight)
        {
            return soundingWeight <= 0
                ? DeploymentMethodType.Wading
                : DeploymentMethodType.Other;
        }

        public static DateTimeInterval GetObservationTimeInterval(DateTimeOffset visitStart, 
            ParsedData parsedData)
        {
            //Some point measurement times can be 00:00. 
            var allNonZeroTimes = parsedData.VerticalObservations
                .SelectMany(v => v.ObservationPoints)
                .Where(p => p.MeasuredAtTime != TimeSpan.Zero)
                .Select(p => p.MeasuredAtTime)
                .ToList();

            var startTime = allNonZeroTimes.Min();
            var endTime = allNonZeroTimes.Max();

            var startDateTimeOffset = new DateTimeOffset(visitStart.Date, visitStart.Offset).Add(startTime);
            var endDateTimeOffset = startDateTimeOffset.Add(endTime - startTime);

            return new DateTimeInterval(startDateTimeOffset, endDateTimeOffset);
        }
    }
}
