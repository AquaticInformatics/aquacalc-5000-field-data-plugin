using System;
using System.Collections.Generic;

namespace AquaCalc5000.Parsers
{
    public class ParsedData
    {
        public string FirmwareVersion { get; set; }

        public string GageId { get; set; }
        public DateTime StartDate { get; set; }
        public int Transect { get; set; }
        public string UserId { get; set; }

        public double StaffStageBegin { get; set; }
        public double StaffStageEnd { get; set; }
        public double LoggerStageBegin { get; set; }
        public double LoggerStageEnd { get; set; }

        public string MeterId { get; set; }
        public double SoundingWeight { get; set; }
        public string StartMode { get; set; }
        public string MeterType { get; set; }
        public double MeterConst1 { get; set; }
        public double MeterConst2 { get; set; }
        public double MeterConst3 { get; set; }
        public double MeterConst4 { get; set; }
        public double MeterConst5 { get; set; }

        public string UnitSystem { get; set; }

        public int TotalVerticals { get; set; }
        public int TotalStations { get; set; }
        public double TotalWidth { get; set; }
        public double TotalArea { get; set; }
        public double TotalDischarge { get; set; }

        public double MeanVelocity { get; set; }

        public List<CsvLine> ObservationSectionLines { get; set; } = new List<CsvLine>();
        public List<VerticalObservation> VerticalObservations { get; set; } = new List<VerticalObservation>();
        public List<CsvLine> ErrorFlagLines { get; set; } = new List<CsvLine>();
    }
}
