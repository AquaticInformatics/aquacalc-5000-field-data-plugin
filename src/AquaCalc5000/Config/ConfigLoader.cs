using System.IO;
using AquaCalc5000.Parsers;

namespace AquaCalc5000.Config
{
    public class ConfigLoader
    {
        public Config Load()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var path = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), Config.ConfigFileName);

            if (!File.Exists(path))
            {
                return new Config();
            }

            var text = File.ReadAllText(path);

            var csvParser = new CsvParser(text, '=');
            var isUsgsSiteId = csvParser.GetRequiredBooleanByLabelOrDefault(
                nameof(Config.AssumeUsgsSiteIdentifiers));

            return new Config {AssumeUsgsSiteIdentifiers = isUsgsSiteId};
        }
    }
}
