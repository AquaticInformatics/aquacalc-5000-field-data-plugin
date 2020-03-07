using AquaCalc5000.Parser;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests
{
    [TestFixture]
    public class AquaCalcCsvParserTests : TestBase
    {
        private const string DefaultAquaCalcTestFileName = "AquaCalc5000Test.csv";
        private ParsedData _parsedData;

        [OneTimeSetUp]
        public void ParseDefaultTestDataOnceForAllTests()
        {
            var stream = GetEmbeddedFileStream(DefaultAquaCalcTestFileName);
            _parsedData = new AquaCalcCsvParser(stream).Parse();
        }

        [TestCase(DefaultAquaCalcTestFileName, true)]
        [TestCase("NotAquaCalc5000.csv", false)]
        public void CanParse_CheckIncomingDataCorrectly(string fileName, bool expectedCanParse)
        {
            var stream = GetEmbeddedFileStream(fileName);
            var parser = new AquaCalcCsvParser(stream);

            Assert.That(parser.CanParse(), Is.EqualTo(expectedCanParse));
        }

        [Test]
        public void HeaderItemsParsedCorrectly()
        {
            Assert.That(_parsedData.LocationIdentifier, Is.EqualTo("6687500"));
        }
    }
}
