using Boyd.NMEA.NMEA.Messages;
using System;
using System.Collections.Generic;
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


        public decimal Latitude => Location.Latitude.Value;
        public decimal Longitude => Location.Longitude.Value;

        public decimal F1Depth => -Depth.F1;
        public decimal F2Depth => -Depth.F2;

        public string LatitudeAsString => $"{Location.Latitude} {Location.GeoidSeperationUnits} {Location.EastWest}";
        public string LongitudeAsString => $"{Location.Longitude} {Location.GeoidSeperationUnits} {Location.NorthSouth}";

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Location != null)
            {
                sb.AppendLine("Location:");
                sb.AppendLine($"    Altitude : {Location.Altitude} {Location.AltitudeUnits}");
                sb.AppendLine($"    Longitude : {Location.Longitude} {Location.GeoidSeperationUnits} {Location.NorthSouth}");
                sb.AppendLine($"    Latitude : {Location.Latitude} {Location.GeoidSeperationUnits} {Location.EastWest}");
                sb.AppendLine($"    Sattelites : {Location.SatsInView}");
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
            return $"{RecordNumber},{Location.Latitude},{Location.GeoidSeperationUnits},{Location.NorthSouth},{Location.Longitude},{Location.GeoidSeperationUnits},{Location.EastWest},{Location.SatsInView},{Depth.F1},{Depth.F2},{Depth.Unit},{RecordTime}";
        }
    }
}
