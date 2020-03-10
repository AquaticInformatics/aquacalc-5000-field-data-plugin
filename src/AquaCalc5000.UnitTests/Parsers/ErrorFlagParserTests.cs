using System;
using System.Collections.Generic;
using System.Linq;
using AquaCalc5000.Parsers;
using NUnit.Framework;

namespace AquaCalc5000.UnitTests.Parsers
{
    [TestFixture]
    public class ErrorFlagParserTests
    {
        private readonly List<CsvLine> _errorFlagLines = new List<CsvLine>
        {
            new CsvLine("1. USER EXCEEDED SINGLE SUBSECTION 05% EST. Q.", 72),
            new CsvLine(" 2. THE PRODUCT OF VELOCITY AND DEPTH EXCEEDED THE", 73),
            new CsvLine("   SELECTED SOUNDING WEIGHT.", 74),
            new CsvLine("3. INCORRECT METER USED FOR DEPTH OF STREAM.", 75),
        };

        [Test]
        public void GetFlagNoteDictionary_ReturnsCorrectMapping()
        {
            var parser = new ErrorFlagParser(_errorFlagLines);
            var dic = parser.FlagNoteDictionary;

            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[2], Is.EqualTo("2. THE PRODUCT OF VELOCITY AND DEPTH EXCEEDED THE SELECTED SOUNDING WEIGHT."));
            Assert.That(dic[3], Is.EqualTo("3. INCORRECT METER USED FOR DEPTH OF STREAM."));
        }

        [Test]
        public void GetNoteByFlags_CombinesSingleNotes()
        {
            var parser = new ErrorFlagParser(_errorFlagLines);
            var flags = new[] {"1", "3"};

            var notes = parser.GetNoteByFlags(flags.ToList());
            Assert.That(notes, Is.EqualTo($"1. USER EXCEEDED SINGLE SUBSECTION 05% EST. Q.{Environment.NewLine}" +
                                          "3. INCORRECT METER USED FOR DEPTH OF STREAM."));
        }

        [TestCase("13")]
        [TestCase("1 3")]
        public void GetNoteByFlags_SplitsMultipleValidFlags(string flagStr)
        {
            var parser = new ErrorFlagParser(_errorFlagLines);
            var flags = new[] { flagStr };

            var notes = parser.GetNoteByFlags(flags.ToList());
            Assert.That(notes, Is.EqualTo($"1. USER EXCEEDED SINGLE SUBSECTION 05% EST. Q.{Environment.NewLine}" +
                                          "3. INCORRECT METER USED FOR DEPTH OF STREAM."));
        }
    }
}
