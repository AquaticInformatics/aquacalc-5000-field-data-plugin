namespace AquaCalc5000.Parsers
{
    public static class AquaCalcConstants
    {
        //Comments are added with reference to:
        //http://www.jbsenergy.com/downloads/AquaCalc_5000_Advanced_Manual_8c.pdf

        //Header section:
        public const string AquaCalc5000 = "AquaCalc 5000";
        public const string FirmwareVersion = "Firmware Version";

        public const string GageId = "GAGE ID#"; //Stream Stage Gage ID
        public const string Date = "DATE";//M/d/yyyy: 4/18/2018 or M/d/yy
        public const string Transect = "TRANSECT"; //1-15 transect# in AquaCalc
        public const string UserId = "USER ID#";
        public const string ShBegin = "SH BEGIN"; //Staff Height Begin: prior to measurement, the stage read from the outside Staff Gage.
        public const string ShEnd = "SH END"; //Staff Height End: upon the completion,the stage read from the outside Staff Gage.
        public const string GhBegin = "GH BEGIN"; //Gage Height Begin: prior to discharge measurement, reported by the data logger.
        public const string GhEnd = "GH END"; //Gage Height End:upon the completion, reported by the data logger.
        public const string EstDischarge = "EST. DISCHARGE";//Estimated Total Q
        public const string EstQ = "EST. Q (ADJ)";//Adjusted Total Q. Seems to be always = Total Discharge.
        public const string MeterId = "METER ID#";//Current meter serial Number or ID
        public const string AquaCalcId = "AQUACALC ID#";//AquaCalc Serial Number
        public const string SoundingWt = "SOUNDING WT.";//Sounding Weight: 0 = wading rod, >0=a suspension measurement. Page 113.
        public const string StartMeasAt = "START MEAS. AT"; //Start mode. LEW: left edge of water; REW: right edge.
        public const string MeterType = "METER TYPE";//E.g.,Price AA 1:1, Price AA 5:1, Pygmy or Non-Standard
        //Meter constants for the standard meters, Price and Pygmy. Page 66 
        public const string MeterConstC1 = "METER CONST. C1";//Slope 1
        public const string MeterConstC2 = "METER CONST. C2";//Intercept 1
        public const string MeterConstC3 = "METER CONST. C3";//Slope 2
        public const string MeterConstC4 = "METER CONST. C4";//Intercept 2
        public const string MeterConstC5 = "METER CONST. C5";//Cross over velocity

        public const string MeasurementTime = "MEASUREMENT TIME";//Time interval (seconds) of rotations (0-99). Default:40 
        public const string MeasSystem = "MEAS. SYSTEM";//Unit system. SAE: Imperial, Society of Automotive Engineers. SI: Metric
        public const string PercentSlope = "PERCENT SLOPE";//slope of the stream to calculate Manning n value.
        public const string TotalVerticals = "TOTAL VERTICALS"; 
        public const string TotalStations = "TOTAL STATIONS";//Total measured points = lines of observations in csv.
        public const string TotalWidth = "TOTAL WIDTH";
        public const string TotalArea = "TOTAL AREA";
        public const string TotalDischarge = "TOTAL DISCHARGE";
        public const string PctDifference = "PCT DIFFERENCE"; //Percent Difference of the Estimated Q
        public const string MeanVelocity = "MEAN VELOCITY";
        public const string WettedPerimeter = "WETTED PERIMETER";
        public const string HydraulicRadius = "HYDRAULIC RADIUS"; //For estimating the average velocity using Manning formula
        public const string ManningFactor = "MANNING FACTOR";

        //Measurement Section Column Descriptions
        public const string ObsSeq = "OB"; //Sequence number of observation
        public const string Distance = "DIST"; //Tag Line Distance
        public const string Depth = "DEPTH"; //Total water depth.
        public const string Ice = "ICE"; //Bottom of ice to water surface.AquaCalc will subtract Ice Draft from total water depth when calculating Q.
        public const string Revolutions = "REVS"; //Revolutions
        public const string Time = "TIME"; //Observation Time in seconds
        public const string CosineVerticalFactor = "COS:VF"; //Cosine Vertical Factor (for getting mean vertical velocity at a vertical wall)
        public const string Loc = "LOC"; //Measured point location: S, 2, 6, 8, B, or W. S=surface, B=Bottom.W=Wall. Page 19.
        public const string Coef = "COEF"; //Coefficient
        public const string Clock = "CLOCK"; //Measured timestamp: HH:MM
        public const string Vel = "VEL"; //Velocity
        public const string Area = "AREA";
        public const string FlowQ = "FLOW(Q)";
        public const string Flags = "FLAGS"; //Error flags: Optionally separated by a space.E.g., "5 8" or 58".
    }
}
