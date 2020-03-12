using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework.DataModel.Meters;
using FieldDataPluginFramework.DataModel.Verticals;

namespace AquaCalc5000.Mappers
{
    public class VerticalMapper
    {
        private readonly ParsedData _parsedData;
        private readonly DateTimeOffset _visitStart;
        public VerticalMapper(ParsedData parsedData, DateTimeOffset visitStart)
        {
            _parsedData = parsedData;
            _visitStart = visitStart;
        }

        public IEnumerable<Vertical> GetVerticals()
        {
            var timeInterval = CommonMapper.GetObservationTimeInterval(_visitStart, _parsedData);

            var verticals = new List<Vertical>();
            var startOfDate = new DateTimeOffset(timeInterval.Start.Date, timeInterval.Start.Offset);

            var calculatedTotalQ = _parsedData.VerticalObservations.Sum(v => v.FlowQSum);
            var meterCalibration = GetMeterCalibration();

            for (var index = 0; index < _parsedData.VerticalObservations.Count; index++)
            {
                var verticalObservation = _parsedData.VerticalObservations[index];
                var previousTaglinePos = index == 0 ? 0 : _parsedData.VerticalObservations[index - 1].Distance;

                var segment = GetSegment(verticalObservation, previousTaglinePos, calculatedTotalQ);
                var sequenceNumber = index + 1;

                var vertical = new Vertical
                {
                    Segment = segment,
                    MeasurementConditionData = GetConditionData(verticalObservation),
                    TaglinePosition = verticalObservation.Distance,
                    SequenceNumber = sequenceNumber,
                    MeasurementTime = startOfDate.Add(verticalObservation.StartTime),
                    VerticalType = GetVerticalTypeOrMidRiver(verticalObservation, sequenceNumber),
                    EffectiveDepth = verticalObservation.Depth,

                    VelocityObservation = GetVelocityObservation(verticalObservation, meterCalibration, sequenceNumber),
                    Comments = verticalObservation.Comments,
                    FlowDirection = FlowDirectionType.Normal,
                    SoundedDepth = verticalObservation.Depth,
                    IsSoundedDepthEstimated = verticalObservation.Comments.Contains("DEPTH ESTIMATED"), 
                    //ObliqueFlowCorrection: N/A
                };

                SetAdditionalVerticalComments(vertical, verticalObservation);

                verticals.Add(vertical);
            }

            return verticals;
        }

        private MeterCalibration GetMeterCalibration()
        {
            var meterTypeString = _parsedData.MeterType;
            var serialNumber = _parsedData.MeterId;

            var manufacturer = AquaCalcConstants.AquaCalc5000;

            var calibration = new MeterCalibration
                {
                    Manufacturer = manufacturer,
                    Model = meterTypeString,
                    //SoftwareVersion: N/A
                    SerialNumber = serialNumber,
                    MeterType = GetMeterTypeEnumOrUnspecified(meterTypeString),
                };

            return calibration;
        }

        private MeterType GetMeterTypeEnumOrUnspecified(string meterTypeString)
        {
            if (string.IsNullOrWhiteSpace(meterTypeString))
                return MeterType.Unspecified;

            if (meterTypeString.IndexOf("Price AA", StringComparison.OrdinalIgnoreCase) >=0 )
                return MeterType.PriceAa;
            if (meterTypeString.IndexOf("Pygmy", StringComparison.OrdinalIgnoreCase) >= 0)
                return MeterType.Pygmy;

            return MeterType.Unspecified;
        }

        private static Segment GetSegment(VerticalObservation observation, double previousTaglinePos,
            double calculatedTotalQ)
        {
            var segmentWidth = observation.Distance - previousTaglinePos;
            var dischargePortion = calculatedTotalQ <= 0.0
                ? 0
                : observation.FlowQSum/ calculatedTotalQ * 100; 

            return new Segment
            {
                Area = observation.AreaSum,
                Discharge = observation.FlowQSum,
                Velocity = observation.MeanVelocity,
                Width = segmentWidth,
                TotalDischargePortion = dischargePortion,
                IsDischargeEstimated = false
            };
        }

        private MeasurementConditionData GetConditionData(VerticalObservation observation)
        {
            if (observation.HasIce)
            {
                return GetIceCoveredData(observation);
            }
            
            return GetOpenWaterData(observation);
        }

        private IceCoveredData GetIceCoveredData(VerticalObservation observation)
        {
            return new IceCoveredData
            {
                IceThickness = observation.IceThickness,
                WaterSurfaceToBottomOfIce = observation.IceThickness,
                WaterSurfaceToBottomOfSlush = observation.IceThickness,
                //UnderIceCoefficient: N/A
                //AboveFooting: N/A
                //BelowFooting: N/A
                //IceAssemblyType: N/A
            };
        }

        private OpenWaterData GetOpenWaterData(VerticalObservation observation)
        {
            return new OpenWaterData
            {
                SuspensionWeight = _parsedData.SoundingWeight > 0 ? $"{_parsedData.SoundingWeight}": null,
                SurfaceCoefficient = observation.Coefficient,
                //DryLineCorrection: N/A
                //WetLineCorrection: N/A
                //DistanceToWaterSurface: N/A
                //DistanceToMeter: N/A
                //DryLineAngle: N/A
            };
        }

        private VelocityObservation GetVelocityObservation(VerticalObservation verticalObservation, 
            MeterCalibration meterCalibration, int sequenceNumber)
        {
            var depthObservations = GetVelocityDepthObservations(verticalObservation.ObservationPoints);

            var velocityObservationMethod = GetVelocityObservationMethodOrUnknown(verticalObservation, sequenceNumber);

            var velocityObservation = new VelocityObservation
            {
                MeterCalibration = meterCalibration,
                DeploymentMethod = CommonMapper.GetDeploymentMethodBySoundingWeight(_parsedData.SoundingWeight),
                VelocityObservationMethod = velocityObservationMethod,
                MeanVelocity = verticalObservation.MeanVelocity
            };

            foreach (var velocityDepthObservation in depthObservations)
            {
                velocityObservation.Observations.Add(velocityDepthObservation);
            }

            return velocityObservation;
        }

        private PointVelocityObservationType GetVelocityObservationMethodOrUnknown(VerticalObservation verticalObservation,
            int sequenceNumber)
        {
            if (verticalObservation.FlowQSum <= 0.0 && 
                (sequenceNumber == 1 || sequenceNumber >= _parsedData.VerticalObservations.Count))
            {
                return PointVelocityObservationType.Surface;
            }

            var indicators = string.Join("", verticalObservation.LocationIndicators.OrderBy(s => s));

            switch (indicators)
            {
                case "6":
                    return PointVelocityObservationType.OneAtPointSix;
                case "2W":
                case "28":
                    return PointVelocityObservationType.OneAtPointTwoAndPointEight;
                case "268":
                    return PointVelocityObservationType.OneAtPointTwoPointSixAndPointEight;
                case "5":
                    return PointVelocityObservationType.OneAtPointFive;
                case "S":
                    return PointVelocityObservationType.Surface;
                case "W": //W, B: No equivalent in AQTS. Will add a comment in the vertical.
                    return PointVelocityObservationType.OneAtPointSix;
                case "B":
                    return PointVelocityObservationType.OneAtPointSix;

                default:
                    return PointVelocityObservationType.Unknown;

            }
        }

        private List<VelocityDepthObservation> GetVelocityDepthObservations(List<ObservationPoint> points)
        {
            return points.Select(point => new VelocityDepthObservation
            {
                Depth = GetActualPointMeasurementDepth(point),
                ObservationInterval = point.TimeSeconds,
                RevolutionCount = point.Revolutions,
                Velocity = point.Velocity,
            }).ToList();
        }

        private double GetActualPointMeasurementDepth(ObservationPoint point)
        {
            if (!double.TryParse(point.ObsLocationIndicator, out double indicatorValue))
            {
                indicatorValue = 6;
            }

            if (point.ObsLocationIndicator == "B")
            {
                indicatorValue = 10;
            }

            return point.Depth * (indicatorValue / 10);
        }

        private VerticalType GetVerticalTypeOrMidRiver(VerticalObservation verticalObservation, int sequenceNumber)
        {
            var isFirstVertical = sequenceNumber == 1;
            if (isFirstVertical && verticalObservation.FlowQSum <= 0)
            {
                return VerticalType.StartEdgeNoWaterBefore;
            }

            var isLastVertical = sequenceNumber == _parsedData.VerticalObservations.Count - 1;
            if (isLastVertical && verticalObservation.FlowQSum <= 0)
            {
                return VerticalType.EndEdgeNoWaterAfter;
            }

            return VerticalType.MidRiver;
        }

        private void SetAdditionalVerticalComments(Vertical vertical, 
            VerticalObservation verticalObservation)
        {
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(vertical.Comments))
            {
                stringBuilder.AppendLine(vertical.Comments);
            }

            if (verticalObservation.LocationIndicators.Count == 1 &&
                verticalObservation.LocationIndicators.First() =="W")
            {
                stringBuilder.AppendLine(CommonMapper.WallMeasurementMappedToPoint6);
            }

            if (verticalObservation.LocationIndicators.Contains("B"))
            {
                stringBuilder.AppendLine(CommonMapper.BottomMeasurementMappedToPoint6);
            }

            var cosVfValues = verticalObservation.ObservationPoints
                .Select(point => point.CosineVerticalFactor)
                .Where(vf => Math.Abs(vf - 1.0d) > 0.0000001)
                .ToList();
            if (cosVfValues.Any())
            {
                stringBuilder.AppendLine($"{CommonMapper.CosVfEquals}{string.Join(",", cosVfValues)}");
            }

            vertical.Comments = stringBuilder.ToString().Trim();
        }
    }
}
