using System;
using System.Linq;
using AquaCalc5000.Parsers;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Parsers
{
    [TestFixture]
    public class ObservationSectionParserTests : TestBase
    {
        [Test]
        public void GetVerticalObservations_ParsesOutCorrectValues()
        {
            ParsedData parsedData;
            using (var stream = GetEmbeddedFileStream(DefaultAquaCalcTestFileName))
            {
                parsedData = new AquaCalcCsvParser(stream).Parse();
            }

            var parser = new ObservationSectionParser(parsedData.ObservationSectionLines);
            var observations = parser.GetVerticals();

            Assert.That(observations.Count, Is.EqualTo(23));

            Assert.That(observations[2].ObservationPoints.Count, Is.EqualTo(2));
            var point = observations[2].ObservationPoints.First();

            Assert.That(point.ObservationNumber, Is.EqualTo(3));
            Assert.That(point.Distance, Is.EqualTo(25.0d));
            Assert.That(point.Depth, Is.EqualTo(3.1d));
            Assert.That(point.IceBottomToWaterSurface, Is.EqualTo(0.0d));
            Assert.That(point.Revolutions, Is.EqualTo(15));
            Assert.That(point.TimeSeconds, Is.EqualTo(20.6d));
            Assert.That(point.CosineVerticalFactor, Is.EqualTo(1.0d));
            Assert.That(point.ObsLocationIndicator, Is.EqualTo("8"));
            Assert.That(point.Coefficient, Is.EqualTo(1.0d));
            Assert.That(point.MeasuredAtTime, Is.EqualTo(new TimeSpan(11, 51, 0)));
            Assert.That(point.Velocity, Is.EqualTo(1.623));
            Assert.That(point.Area, Is.EqualTo(15.5));
            Assert.That(point.FlowQ, Is.EqualTo(24.44));
            Assert.That(point.Flags, Is.EqualTo("5"));
        }
    }
}
