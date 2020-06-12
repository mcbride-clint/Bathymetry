using System;
using System.Text;

namespace Bathymetry.Data.Models
{
    public class Reading
    {
        public Reading()
        {
            RecordTime = DateTime.Now;
        }

        public DepthInfo Depth { get; set; }
        public GGAMessage Location { get; set; }
        public int RecordNumber { get; set; }
        public DateTime RecordTime { get; set; }
        public bool IsValid => (Depth != null && Location != null);


        public double Latitude => Location.Latitude;
        public double Longitude => Location.Longitude;

        public decimal F1Depth => -Depth.F1;
        public decimal F2Depth => -Depth.F2;

        public string LatitudeAsString => $"{Location.Latitude} {Location.GeoidalSeparationUnits}";
        public string LongitudeAsString => $"{Location.Longitude} {Location.GeoidalSeparationUnits}";

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Location != null)
            {
                sb.AppendLine("Location:");
                sb.AppendLine($"    Altitude : {Location.Altitude} {Location.AltitudeUnits}");
                sb.AppendLine($"    Longitude : {Location.Longitude} {Location.GeoidalSeparationUnits}");
                sb.AppendLine($"    Latitude : {Location.Latitude} {Location.GeoidalSeparationUnits}");
                sb.AppendLine($"    Sattelites : {Location.NumberOfSatellites}");
            }

            if(Depth != null)
            {
                sb.AppendLine("Reading:");
                sb.AppendLine($"    F1 : {Depth.F1} {Depth.Unit}");
                sb.AppendLine($"    F2 : {Depth.F2} {Depth.Unit}");
            }

            sb.AppendLine("--------------------------");
            sb.AppendLine("--------------------------");

            return sb.ToString();
        }

        public string ToCsv()
        { 
            return $"{RecordNumber},{Location.Latitude},{Location.GeoidalSeparationUnits},{Location.Longitude},{Location.GeoidalSeparationUnits},{Location.NumberOfSatellites},{Depth.F1},{Depth.F2},{Depth.Unit},{RecordTime}";
        }
    }
}
