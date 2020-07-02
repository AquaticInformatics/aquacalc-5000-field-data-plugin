using System.Collections.Generic;

namespace AquaCalc5000.Config
{
    public class ConfigLoader
    {
        public Config Load(Dictionary<string, string> settings)
        {
            if (settings != null
                && settings.TryGetValue(nameof(Config.AssumeUsgsSiteIdentifiers), out var text)
                && bool.TryParse(text, out var assumeUsgsSiteIdentifiers))
            {
                return new Config
                {
                    AssumeUsgsSiteIdentifiers = assumeUsgsSiteIdentifiers
                };
            }

            return new Config();
        }
    }
}
