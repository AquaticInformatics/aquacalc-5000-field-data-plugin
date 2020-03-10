using System.Collections.Generic;
using System.Linq;

namespace AquaCalc5000.Parsers
{
    public class VerticalObservation
    {
        public List<ObservationPoint> ObservationPoints { get; set; } = new List<ObservationPoint>();

        public double Distance => ObservationPoints.Any() ? ObservationPoints.First().Distance : 0.0;
        public double Depth => ObservationPoints.Any() ? ObservationPoints.First().Depth : 0.0;
        public List<string> Flags => ObservationPoints
            .Where(obs => !string.IsNullOrWhiteSpace(obs.Flags))
            .Select(obs => obs.Flags)
            .ToList();

        public string Comments { get; set; }
    }
}
