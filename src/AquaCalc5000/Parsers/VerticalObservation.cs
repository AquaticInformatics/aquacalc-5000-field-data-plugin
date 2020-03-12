using System;
using System.Collections.Generic;
using System.Linq;

namespace AquaCalc5000.Parsers
{
    public class VerticalObservation
    {
        public List<ObservationPoint> ObservationPoints { get; set; } = new List<ObservationPoint>();

        //Same distance on all points:
        public double Distance => ObservationPoints.Average(point => point.Distance);

        //Same depth for all points:
        public double Depth
        {
            get
            {
                var averageDepth = ObservationPoints.Average(point => point.Depth);
                if (averageDepth <= 0 && IceThickness > 0)
                {
                    averageDepth = IceThickness;
                }

                return averageDepth;
            }
        }

        //Normally only one point has the value and other points will have 0.
        public double AreaSum => ObservationPoints.Sum(point => point.Area);

        public double FlowQSum => ObservationPoints.Sum(point => point.FlowQ);

        public double MeanVelocity => ObservationPoints.Average(point => point.Velocity);

        public TimeSpan StartTime => ObservationPoints.Any() 
            ? ObservationPoints.First().MeasuredAtTime 
            : TimeSpan.Zero;

        //Same ice thickness for all points:
        public double IceThickness => ObservationPoints.Average(point => point.IceBottomToWaterSurface);

        public bool HasIce => IceThickness > 0;

        //Should be same for all points. Default: 1
        public double Coefficient => ObservationPoints.Any() 
            ? ObservationPoints.First().Coefficient 
            : 1;

        public List<string> Flags => ObservationPoints
            .Where(obs => !string.IsNullOrWhiteSpace(obs.Flags))
            .Select(obs => obs.Flags)
            .ToList();

        public List<string> LocationIndicators =>
            ObservationPoints.Select(point => point.ObsLocationIndicator).Distinct().ToList();

        public string Comments { get; set; }
    }
}
