﻿using System;
using System.Linq;
using AquaCalc5000.Parsers;
using AquaCalc5000.SystemCode;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.Units;

namespace AquaCalc5000.Mappers
{
    public class CommonMapper
    {
        public const string WallMeasurementMappedToPoint6 = "Wall measurement. Mapped to .6."; 
        public const string BottomMeasurementMappedToPoint6 = "Bottom measurement. Mapped to .6.";
        public const string CosVfEquals = "COS:VF=";
        public const string Location6AdjustedTo2DueToWallMeasurement = "Location 6 adjusted to 2 due to wall measurement.";

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

        public static UnitSystem GetUnitSystem(ParsedData parsedData)
        {
            var unitSystemCode = parsedData.UnitSystem;

            switch (unitSystemCode)
            {
                case "SAE":
                    return Units.ImperialUnitSystem;
                case "SI":
                    return Units.MetricUnitSystem;
                default:
                    throw new ArgumentException($"Unknown unit system code:'{unitSystemCode}'");
            }
        }
    }
}
