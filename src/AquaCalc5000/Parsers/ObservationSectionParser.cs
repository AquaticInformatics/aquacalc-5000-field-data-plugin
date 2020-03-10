using System;
using System.Collections.Generic;
using System.Linq;

namespace AquaCalc5000.Parsers
{
    public class ObservationSectionParser
    {
        private readonly List<CsvLine> _obsCsvLines;

        public ObservationSectionParser(List<CsvLine> obsCsvLines)
        {
            _obsCsvLines = obsCsvLines;
        }

        public List<VerticalObservation> GetVerticals()
        {
            var headerLine = _obsCsvLines.FirstOrDefault(l =>
                l.Parts.Any(s => s == AquaCalcConstants.ObsSeq) &&
                l.Parts.Any(s => s == AquaCalcConstants.Distance));
            if (headerLine == null)
            {
                throw new ArgumentException($"Missing header line for observation section.");
            }

            var headers = headerLine.Parts.Select((header, index) => new { header, index })
                .ToDictionary(item => item.header, item => item.index);

            var obsPointLines = new List<CsvLine>(_obsCsvLines);
            obsPointLines.Remove(headerLine);

            var observationPoints = GetObservationPoints(headers, obsPointLines);

            var observationGroups = observationPoints.GroupBy(p => new { p.Depth, p.Distance }).ToList();

            return observationGroups.Select(g => new VerticalObservation { ObservationPoints = g.ToList() }).ToList();
        }

        private List<ObservationPoint> GetObservationPoints(IDictionary<string, int> headers, List<CsvLine> obsPointLines)
        {
            var observationPoints = new List<ObservationPoint>();

            foreach (var line in obsPointLines)
            {
                var parts = line.Parts;
                var point = new ObservationPoint
                {
                    ObservationNumber = int.Parse(parts[headers[AquaCalcConstants.ObsSeq]]),
                    Distance = double.Parse(parts[headers[AquaCalcConstants.Distance]]),
                    Depth = double.Parse(parts[headers[AquaCalcConstants.Depth]]),
                    IceBottomToWaterSurface = double.Parse(parts[headers[AquaCalcConstants.Ice]]),
                    Revolutions = int.Parse(parts[headers[AquaCalcConstants.Revolutions]]),
                    TimeSeconds = double.Parse(parts[headers[AquaCalcConstants.Time]]),
                    CosineVerticalFactor = double.Parse(parts[headers[AquaCalcConstants.CosineVerticalFactor]]),
                    ObsLocationIndicator = parts[headers[AquaCalcConstants.Loc]],
                    Coefficient = double.Parse(parts[headers[AquaCalcConstants.Coef]]),
                    MeasuredAtTime = TimeSpan.Parse(parts[headers[AquaCalcConstants.Clock]]),
                    Velocity = double.Parse(parts[headers[AquaCalcConstants.Vel]]),
                    Area = double.Parse(parts[headers[AquaCalcConstants.Area]]),
                    FlowQ = double.Parse(parts[headers[AquaCalcConstants.FlowQ]]),
                    Flags = parts[headers[AquaCalcConstants.Flags]]
                };

                observationPoints.Add(point);
            }

            return observationPoints;
        }
    }
}
