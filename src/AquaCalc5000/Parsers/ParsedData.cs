using System;
using System.Collections.Generic;

namespace AquaCalc5000.Parsers
{
    public class ParsedData
    {
        public string LocationIdentifier { get; set; }
        public DateTime StartDate { get; set; }
        public int Transect { get; set; }
        public int TotalVerticals { get; set; }
        public int TotalStations { get; set; }

        public List<CsvLine> ObservationSectionLines { get; set; } = new List<CsvLine>();
        public List<VerticalObservation> VerticalObservations { get; set; } = new List<VerticalObservation>();
        public List<CsvLine> ErrorFlagLines { get; set; } = new List<CsvLine>();
    }
}
