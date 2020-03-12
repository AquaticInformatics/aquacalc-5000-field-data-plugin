using System;
using System.Linq;
using AquaCalc5000.Mappers;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Mappers
{
    [TestFixture]
    public class ActivityMapperTests : TestBase
    {
        private ParsedData _parsedData;
        private DateTimeInterval _visitInterval;

        [SetUp]
        public void InitializeBeforeEachTest()
        {
            using (var stream = GetEmbeddedFileStream(DefaultAquaCalcTestFileName))
            {
                _parsedData = new AquaCalcCsvParser(stream).Parse();
                var measurementStart = new DateTimeOffset(2018, 01, 03, 0, 0, 0, TimeSpan.FromHours(-8));
                _visitInterval = new DateTimeInterval(measurementStart, TimeSpan.Parse("13:25"));
            }
        }

        [Test]
        public void GetDischargeActivity_ReturnsCorrectDischargeAndGageHeights()
        {
            var parsedActivity = new ActivityMapper(_parsedData, _visitInterval).GetDischargeActivity();
            Assert.That(parsedActivity.Discharge.Value, Is.EqualTo(1190.0d));

            Assert.That(parsedActivity.GageHeightMeasurements.Count, Is.EqualTo(4));

            var firstGageMeasurement = parsedActivity.GageHeightMeasurements.First();
            Assert.That(firstGageMeasurement.GageHeight.Value, Is.EqualTo(8.11));
            Assert.That(firstGageMeasurement.MeasurementTime, 
                Is.EqualTo(_visitInterval.Start.Add(TimeSpan.Parse("11:50"))));
        }

        [TestCase("SAE", "ft")]
        [TestCase("SI", "m")]
        public void GetDischargeActivity_SetsCorrectUnitSystem(string unitSystemCode, string expectedLengthUnitId)
        {
            _parsedData.UnitSystem = unitSystemCode;
            var dischargeSection = GetParsedDischargeSection(_parsedData);

            Assert.That(dischargeSection.Width.UnitId, Is.EqualTo(expectedLengthUnitId));
        }

        private ManualGaugingDischargeSection GetParsedDischargeSection(ParsedData parsedData)
        {
            var parsedActivity = new ActivityMapper(parsedData, _visitInterval).GetDischargeActivity();
            return (ManualGaugingDischargeSection) parsedActivity.ChannelMeasurements.First();
        }

        [TestCase("LEW", StartPointType.LeftEdgeOfWater)]
        [TestCase("REW", StartPointType.RightEdgeOfWater)]
        public void GetDischargeActivity_SetsCorrectStartPointType(string startMode, StartPointType expectedStartPoint)
        {
            _parsedData.StartMode = startMode;
            var dischargeSection = GetParsedDischargeSection(_parsedData);

            Assert.That(dischargeSection.StartPoint, Is.EqualTo(expectedStartPoint));
        }

        [TestCase(0, DeploymentMethodType.Wading, MeterSuspensionType.RoundRod)]
        [TestCase(11.5, DeploymentMethodType.Other, MeterSuspensionType.Unknown)]
        public void GetDischargeActivity_SetsCorrectTechnologyInfo(double soundingWeight, 
            DeploymentMethodType expectedMethod, MeterSuspensionType expectedSuspension)
        {
            _parsedData.SoundingWeight = soundingWeight;
            var dischargeSection = GetParsedDischargeSection(_parsedData);

            Assert.That(dischargeSection.DeploymentMethod, Is.EqualTo(expectedMethod));
            Assert.That(dischargeSection.MeterSuspension, Is.EqualTo(expectedSuspension));
        }

        [Test]
        public void GetDischargeActivity_SetsCorrectChannelObservations()
        {
            var dischargeSection = GetParsedDischargeSection(_parsedData);

            Assert.That(dischargeSection.Width.Value, Is.EqualTo(289.00d));
            Assert.That(dischargeSection.Area.Value, Is.EqualTo(676.00d));
            Assert.That(dischargeSection.VelocityAverage.Value, Is.EqualTo(1.76));
        }

        [Test]
        public void GetDischargeActivity_SetsCorrectNumberOfVerticals()
        {
            var dischargeSection = GetParsedDischargeSection(_parsedData);

            Assert.That(dischargeSection.Verticals.Count, Is.EqualTo(_parsedData.TotalVerticals));
        }
    }
}
