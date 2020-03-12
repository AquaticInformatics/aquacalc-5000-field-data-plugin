using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AquaCalc5000.Parsers
{
    public class ErrorFlagParser
    {
        private readonly List<CsvLine> _sortedLines;
        private static readonly Regex FlagLineRegEx = new Regex(@"^(?<Flag>\d+)\.(?<Note>.+)", RegexOptions.Compiled);

        public IReadOnlyDictionary<int,string> FlagNoteDictionary { get; set; }

        public ErrorFlagParser(List<CsvLine> errorFlagLines)
        {
            _sortedLines = GetValidatedSortedLines(errorFlagLines);
            FlagNoteDictionary = GetFlagNoteDictionary();
        }

        private List<CsvLine> GetValidatedSortedLines(List<CsvLine> lines)
        {
            if (!lines.Any())
            {
                return new List<CsvLine>();
            }

            var firstLine = lines.First().OriginalLine;
            if (!FlagLineRegEx.IsMatch(firstLine))
            {
                throw new ArgumentException($"Invalid first line in error flag footer:'{firstLine}'");
            }

            return lines.OrderBy(l => l.LineNumber).ToList();
        }

        private IReadOnlyDictionary<int, string> GetFlagNoteDictionary()
        {
            var dic = new Dictionary<int, string>();
            if (!_sortedLines.Any())
            {
                return dic;
            }

            foreach (var csvLine in _sortedLines)
            {
                var trimmedLine = csvLine.OriginalLine?.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                var match = FlagLineRegEx.Match(trimmedLine);
                if (match.Success)
                {
                    dic.Add(int.Parse(match.Groups["Flag"].Value), trimmedLine);
                }
                else
                {
                    var appendLine = $"{dic.Last().Value} {trimmedLine}";
                    dic[dic.Last().Key] = appendLine;
                }
            }

            return dic;
        }

        public string GetNoteByFlags(List<string> flags)
        {
            var notes = new List<string>();

            var splitFlags = SplitFlags(flags);

            foreach (var flagStr in splitFlags)
            {
                if (!int.TryParse(flagStr, out int flag) || 
                    !FlagNoteDictionary.TryGetValue(flag, out string note))
                {
                    throw new ArgumentException($"Not a valid error flag: '{flagStr}'.");
                }

                if (notes.Contains(note))
                {
                    continue;
                }

                notes.Add(note);
            }

            return string.Join(Environment.NewLine, notes);
        }

        private IEnumerable<string> SplitFlags(List<string> flags)
        {
            var validFlagStrList = FlagNoteDictionary.Keys.Select(k => k.ToString()).ToList();
            var spaceSplitFlags = flags.SelectMany(s => s.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries));

            var splitFlags = new List<string>();
            foreach (var flagStr in spaceSplitFlags)
            {
                if (string.IsNullOrWhiteSpace(flagStr))
                {
                    continue;
                }

                if (!validFlagStrList.Contains(flagStr))
                {
                    //Some observation line has two flags, separated with/without a space. e.g., 34
                    var parts = flagStr.ToCharArray();
                    splitFlags.AddRange(parts.Select(p => p.ToString()));
                    continue;
                }

                splitFlags.Add(flagStr);
            }

            return splitFlags;
        }
    }
}
