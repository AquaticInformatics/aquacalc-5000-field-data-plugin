using System;
using AquaCalc5000.Mappers;
using AquaCalc5000.Parsers;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Mappers
{
    [TestFixture]
    public class VisitMapperTests : TestBase
    {
        private ParsedData _parsedData;

        [OneTimeSetUp]
        public void ParseDefaultTestDataOnceForAllTests()
        {
            using (var stream = GetEmbeddedFileStream(DefaultAquaCalcTestFileName))
            {
                _parsedData = new AquaCalcCsvParser(stream).Parse();
            }
        }

        [Test]
        public void GetVisitDetails_ReturnsCorrectVisitDateAndParty()
        {
            var mapper = new VisitMapper(_parsedData);
            var utcOffset = TimeSpan.FromHours(-8);
            var visitDetails = mapper.GetVisitDetails(utcOffset);

            Assert.That(visitDetails.StartDate, Is.EqualTo(new DateTimeOffset(2018,01,03,0,0,0, utcOffset)));
            Assert.That(visitDetails.Party, Is.EqualTo("1482"));
        }
    }
}
