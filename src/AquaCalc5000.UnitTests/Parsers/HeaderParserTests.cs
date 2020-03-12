using System;
using AquaCalc5000.Parsers;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Parsers
{
    [TestFixture]
    public class HeaderParserTests : TestBase
    {
        [Test]
        public void HeaderItemsParsedCorrectly()
        {
            var csvText = GetEmbeddedFileFullText(DefaultAquaCalcTestFileName);
            var csvParser = new CsvParser(csvText);
            var parsedData = new HeaderParser(csvParser).Parse();

            Assert.That(parsedData.LocationIdentifier, Is.EqualTo("6687500"));
            Assert.That(parsedData.StartDate, Is.EqualTo(new DateTime(2018, 01, 03)));
            Assert.That(parsedData.Transect, Is.EqualTo(1));
            Assert.That(parsedData.UserId, Is.EqualTo("1482"));

            Assert.That(parsedData.StaffStageBegin, Is.EqualTo(8.11));
            Assert.That(parsedData.StaffStageEnd, Is.EqualTo(8.12));
            Assert.That(parsedData.LoggerStageBegin, Is.EqualTo(8.13));
            Assert.That(parsedData.LoggerStageEnd, Is.EqualTo(8.14));

            Assert.That(parsedData.MeterId, Is.EqualTo("121"));
            Assert.That(parsedData.SoundingWeight, Is.EqualTo(11.5));
            Assert.That(parsedData.StartMode, Is.EqualTo("LEW"));
            Assert.That(parsedData.MeterType, Is.EqualTo("Price AA 1:1 ST2"));

            Assert.That(parsedData.UnitSystem, Is.EqualTo("SAE"));

            Assert.That(parsedData.TotalVerticals, Is.EqualTo(23));
            Assert.That(parsedData.TotalStations, Is.EqualTo(30));
            Assert.That(parsedData.TotalWidth, Is.EqualTo(289.00d));
            Assert.That(parsedData.TotalArea, Is.EqualTo(676.00d));
            Assert.That(parsedData.TotalDischarge, Is.EqualTo(1190.0d));

            Assert.That(parsedData.MeanVelocity, Is.EqualTo(1.76));

        }

        [Test]
        public void GetVisitDate_AdjustsYearToCurrentCenturyIfOlderThan1980()
        {
            var csvParser = new CsvParser("            DATE  ,01/03/1918");
            var parser = new HeaderParser(csvParser);

            var visitDate = parser.GetVisitDate();

            Assert.That(visitDate.Year, Is.EqualTo(2018));
        }
    }
}
