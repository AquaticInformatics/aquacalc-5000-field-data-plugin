using System.Linq;
using AquaCalc5000.Parser;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests
{
    [TestFixture]
    public class CsvParserTests
    {
        [Test]
        public void ShouldTrimParts()
        {
            var parser = new CsvParser("        USER ID#  ,1482");
            var parts = parser.GetFirstNonEmptyLineOrNull().Parts;

            Assert.That(parts.First(), Is.EqualTo("USER ID#"));
            Assert.That(parts.Last(), Is.EqualTo("1482"));
        }

        [TestCase("        USER ID#  ,1482", "USER ID#", "1482")]
        [TestCase("       MEAS. SYSTEM  ,SAE,", "MEAS. SYSTEM", "SAE")]
        [TestCase("       MEAS. SYSTEM  ,SAE,ZZZ", "MEAS. SYSTEM", "SAE,ZZZ")]
        public void GetRequiredStringByLabel_ReturnsCorrectValueIfLabelAndValueExists(string csv,
            string label, string expected)
        {
            var parser = new CsvParser(csv);
            var actualValue = parser.GetRequiredStringByLabel(label);

            Assert.That(actualValue, Is.EqualTo(expected));
        }

        [TestCase("        USER ID#  ,", "USER ID#")]
        [TestCase("       MEAS. SYSTEM  ,SAE,", "USER ID#")]
        public void GetRequiredStringValueByLabel_ThrowsIfValueNotFound(string csv, string label)
        {
            var parser = new CsvParser(csv);
            Assert.That(() => parser.GetRequiredStringByLabel(label), Throws.ArgumentException);
        }
    }
}
