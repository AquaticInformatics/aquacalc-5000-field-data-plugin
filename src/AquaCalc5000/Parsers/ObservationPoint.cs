using System;

namespace AquaCalc5000.Parsers
{
    public class ObservationPoint
    {
        public int ObservationNumber { get; set; }
        public double Distance { get; set; }
        public double Depth { get; set; }
        public double IceBottomToWaterSurface { get; set; }
        public int Revolutions { get; set; }
        public double TimeSeconds { get; set; }
        public double CosineVerticalFactor { get; set; }
        public string ObsLocationIndicator { get; set; }
        public double Coefficient { get; set; } = 1;
        public TimeSpan MeasuredAtTime { get; set; } = TimeSpan.Zero;
        public double Velocity { get; set; }
        public double Area { get; set; }
        public double FlowQ { get; set; }
        public string Flags { get; set; }
    }
}
