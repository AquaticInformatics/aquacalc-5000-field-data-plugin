using System;
using System.Collections.Generic;
using System.Linq;
using AquaCalc5000.Mappers;

namespace AquaCalc5000.Parsers
{
    public class ObservationSectionParser
    {
        private readonly List<CsvLine> _obsCsvLines;

        public ObservationSectionParser(List<CsvLine> obsCsvLines)
        {
            _obsCsvLines = obsCsvLines;
        }

        public List<VerticalObservation> GetVerticalObservations()
        {
            var headerLine = _obsCsvLines.FirstOrDefault(l =>
                l.Parts.Any(s => s == AquaCalcConstants.ObsSeq) &&
                l.Parts.Any(s => s == AquaCalcConstants.Distance));
            if (headerLine == null)
            {
                throw new ArgumentException("Missing header line for observation section.");
            }

            var headers = headerLine.Parts.Select((header, index) => new { header, index })
                .ToDictionary(item => item.header, item => item.index);

            var obsPointLines = new List<CsvLine>(_obsCsvLines);
            obsPointLines.Remove(headerLine);

            var observationPoints = GetObservationPoints(headers, obsPointLines);

            var observationGroups = observationPoints.GroupBy(p => new { p.Distance }).ToList();

            var verticalObservations = observationGroups.Select(g => new VerticalObservation { ObservationPoints = g.ToList()}).ToList();
            CleanupObservationPoints(verticalObservations);

            return verticalObservations;
        }

        private static void CleanupObservationPoints(List<VerticalObservation> verticalObservations)
        {
            foreach (var observation in verticalObservations)
            {
                AdjustLocationsWithWall(observation);
                RemoveDuplicateSurfacePoints(observation);
            }
        }

        private static void AdjustLocationsWithWall(VerticalObservation observation)
        {
            var points = observation.ObservationPoints;
            var locations = string.Join("", observation.LocationIndicators.OrderBy(loc => loc));

            if (points.Count == 2 && locations == "6W")
            {
                var location6Point = points.First(point => point.ObsLocationIndicator == "6");
                location6Point.ObsLocationIndicator = "2";
                observation.Comments = CommonMapper.Location6AdjustedTo2DueToWallMeasurement;
            }
        }

        private static void RemoveDuplicateSurfacePoints(VerticalObservation observation)
        {
            var points = observation.ObservationPoints;
            var surfacePoints = points.Where(p => p.Distance <= 0 &&
                                                  p.Depth <= 0 &&
                                                  p.TimeSeconds <= 0 &&
                                                  p.FlowQ <= 0 &&
                                                  p.Area <= 0).ToList();
            if (points.Count == surfacePoints.Count)
            {
                observation.ObservationPoints = points.Take(1).ToList();
            }
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
