using AquaCalc5000.Parsers;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Parsers
{
    [TestFixture]
    public class AquaCalcCsvParserTests : TestBase
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

        [TestCase(DefaultAquaCalcTestFileName, true)]
        [TestCase("NotAquaCalc5000.csv", false)]
        public void CanParse_CheckIncomingDataCorrectly(string fileName, bool expectedCanParse)
        {
            var stream = GetEmbeddedFileStream(fileName);
            var parser = new AquaCalcCsvParser(stream);

            Assert.That(parser.CanParse(), Is.EqualTo(expectedCanParse));
        }

        [Test]
        public void CorrectNumberOfObservationsParsedCorrectly()
        {
            Assert.That(_parsedData.ObservationSectionLines.Count, Is.EqualTo(31));
            Assert.That(_parsedData.VerticalObservations.Count, Is.EqualTo(23));
        }

        [Test]
        public void CorrectNumberOfLinesInErrorFlagFooterIsParsedOutCorrectly()
        {
            Assert.That(_parsedData.ErrorFlagLines.Count, Is.EqualTo(9));
        }

        [Test]
        public void ErrorFlagsShouldBeParsedOutAsObservationComments()
        {
            Assert.That(_parsedData.VerticalObservations[2].Comments, 
                Is.EqualTo("5. ABNORMAL VELOCITY PROFILE CALCULATED."));
        }
    }
}
