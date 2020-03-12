using System;
using AquaCalc5000.Parsers;
using AquaCalc5000.SystemCode;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.Units;

namespace AquaCalc5000.Mappers
{
    public class ActivityMapper
    {
        private readonly ParsedData _parsedData;
        private readonly DateTimeInterval _visitInterval;

        public ActivityMapper(ParsedData parsedData, DateTimeInterval visitInterval)
        {
            _parsedData = parsedData;
            _visitInterval = visitInterval;
        }

        public DischargeActivity GetDischargeActivity()
        {
            var unitSystem = GetUnitSystem();
            var observationInterval = CommonMapper.GetObservationTimeInterval(_visitInterval.Start,
                _parsedData);

            var dischargeActivity = CreateDischargeActivityWithSummary(observationInterval, unitSystem);

            SetManualGaugingSection(dischargeActivity, unitSystem, observationInterval);
            
            return dischargeActivity;
        }

        private UnitSystem GetUnitSystem()
        {
            var unitSystemCode = _parsedData.UnitSystem;

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

        private DischargeActivity CreateDischargeActivityWithSummary(DateTimeInterval observationInterval, UnitSystem unitSystem)
        {
            var factory = new DischargeActivityFactory(unitSystem);

            var dischargeActivity = factory.CreateDischargeActivity(observationInterval, _parsedData.TotalDischarge);
            dischargeActivity.Party = _parsedData.UserId;

            AddGageHeights(dischargeActivity, observationInterval, unitSystem);

            return dischargeActivity;
        }

        private void AddGageHeights(DischargeActivity dischargeActivity, DateTimeInterval observationInterval,
            UnitSystem unitSystem)
        {
            var staffStageBegin = new Measurement(_parsedData.StaffStageBegin, unitSystem.DistanceUnitId);
            var staffStageEnd = new Measurement(_parsedData.StaffStageEnd, unitSystem.DistanceUnitId);
            var loggerStageBegin = new Measurement(_parsedData.LoggerStageBegin, unitSystem.DistanceUnitId);
            var loggerStageEnd = new Measurement(_parsedData.LoggerStageEnd, unitSystem.DistanceUnitId);

            const bool included = true;

            var startTime = observationInterval.Start;
            var endTime = observationInterval.End;

            dischargeActivity.GageHeightMeasurements.Add(new GageHeightMeasurement(staffStageBegin, startTime, included));
            dischargeActivity.GageHeightMeasurements.Add(new GageHeightMeasurement(staffStageEnd, endTime, included));
            dischargeActivity.GageHeightMeasurements.Add(new GageHeightMeasurement(loggerStageBegin, startTime, included));
            dischargeActivity.GageHeightMeasurements.Add(new GageHeightMeasurement(loggerStageEnd, endTime, included));
        }

        private void SetManualGaugingSection(DischargeActivity dischargeActivity, UnitSystem unitSystem,
            DateTimeInterval observationTimeInterval)
        {
            var dischargeSection = CreateDischargeSectionWithDescription(unitSystem, observationTimeInterval);

            SetDischargeCalculationTechnique(dischargeSection);

            SetTechnologyInfo(dischargeSection);

            SetChannelObservations(dischargeSection, unitSystem);

            SetVerticals(dischargeSection);

            dischargeActivity.ChannelMeasurements.Add(dischargeSection);
            
        }

        private ManualGaugingDischargeSection CreateDischargeSectionWithDescription(UnitSystem unitSystem, 
            DateTimeInterval observationInterval)
        {
            var factory = new ManualGaugingDischargeSectionFactory(unitSystem);
            var manualGaugingDischarge = factory.CreateManualGaugingDischargeSection(observationInterval, _parsedData.TotalDischarge);

            //Party: 
            manualGaugingDischarge.Party = _parsedData.UserId;

            //Discharge Method should always be Mid Section, i.e., points are rendered in the middle of a segment on UI.
            manualGaugingDischarge.DischargeMethod = DischargeMethodType.MidSection; 

            return manualGaugingDischarge;
        }

        private void SetDischargeCalculationTechnique(ManualGaugingDischargeSection dischargeSection)
        {
            //Starting Point:
            switch (_parsedData.StartMode)
            {
                case "LEW":
                    dischargeSection.StartPoint = StartPointType.LeftEdgeOfWater;
                    break;
                case "REW":
                    dischargeSection.StartPoint = StartPointType.RightEdgeOfWater;
                    break;
                default:
                    throw new ArgumentException($"Unknown start point:'{_parsedData.StartMode}'");
            }

            //Other fields in the Technique section on UI will be inferred from the input.
        }

        private void SetTechnologyInfo(ManualGaugingDischargeSection dischargeSection)
        {
            //Primary Current Meter: will be inferred from verticals.

            //Deployment method:
            dischargeSection.DeploymentMethod =
                CommonMapper.GetDeploymentMethodBySoundingWeight(_parsedData.SoundingWeight);

            //Meter Suspension:
            if (dischargeSection.DeploymentMethod == DeploymentMethodType.Wading)
            {
                dischargeSection.MeterSuspension = MeterSuspensionType.RoundRod;
            }

            //Suspension weight: in vertical's OpenWaterData.
        }

        private void SetChannelObservations(ManualGaugingDischargeSection dischargeSection, UnitSystem unitSystem)
        {
            //Width:
            dischargeSection.WidthValue = _parsedData.TotalWidth;
            dischargeSection.DistanceUnitId = unitSystem.DistanceUnitId;

            //River area:
            dischargeSection.AreaValue = _parsedData.TotalArea;
            dischargeSection.AreaUnitId = unitSystem.AreaUnitId;

            //Velocity:
            dischargeSection.VelocityAverageValue = _parsedData.MeanVelocity;
            dischargeSection.VelocityUnitId = unitSystem.VelocityUnitId;
        }

        private void SetVerticals(ManualGaugingDischargeSection dischargeSection)
        {
            var verticals = new VerticalMapper(_parsedData,_visitInterval.Start).GetVerticals();
            foreach (var vertical in verticals)
            {
                dischargeSection.Verticals.Add(vertical);
            }
        }
    }
}
