using Boyd.NMEA.NMEA.Messages;
using System.Collections.Generic;
using System.Text;

namespace Bathymetry.Data.Models
{
    public class Reading
    {
        public DepthInfo Depth { get; set; }
        public GGAMessage Location { get; set; }
        public bool IsValid => (Depth != null && Location != null);

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
    }
}
