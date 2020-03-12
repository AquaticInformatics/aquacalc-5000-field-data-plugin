using System;
using System.Linq;
using AquaCalc5000.Mappers;
using AquaCalc5000.Parsers;
using FieldDataPluginFramework.DataModel;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Mappers
{
    [TestFixture]
    public class VerticalMapperTests : TestBase
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

        [TestCase("W", CommonMapper.WallMeasurementMappedToPoint6)]
        [TestCase("B", CommonMapper.BottomMeasurementMappedToPoint6)]
        public void GetVerticals_AddCommentsOnWallOrBottomMeasurements(string indicator, string expectedComment)
        {
            _parsedData.VerticalObservations[1].ObservationPoints[0].ObsLocationIndicator = indicator;
            var verticals = new VerticalMapper(_parsedData, _visitInterval.Start).GetVerticals().ToList();

            Assert.That(verticals[1].Comments, Contains.Substring(expectedComment));
        }

        [Test]
        public void GetVerticals_AddCommentsIfCosineVerticalFactorIsNot1()
        {
            _parsedData.VerticalObservations[1].ObservationPoints[0].CosineVerticalFactor = 0.85;
            var verticals = new VerticalMapper(_parsedData, _visitInterval.Start).GetVerticals().ToList();

            Assert.That(verticals[1].Comments, Contains.Substring(CommonMapper.CosVfEquals));
        }

    }
}
