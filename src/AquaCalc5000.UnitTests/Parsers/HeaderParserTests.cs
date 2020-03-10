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
            Assert.That(parsedData.TotalStations, Is.EqualTo(30));
            Assert.That(parsedData.TotalVerticals, Is.EqualTo(23));
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
