using System;
using System.Globalization;

namespace Bathymetry.Data
{
    public class GpggaNmeaParser
    {
        public GGAMessage Parse(string gpggaLine)
        {
            var message = gpggaLine.Split(',');

            if (message == null || message.Length < 14)
                throw new ArgumentException("Invalid GGA", "message");

            try
            {
                var gps = new GGAMessage();
                gps.TimeStamp = StringToTimeSpan(message[1]);
                gps.Latitude = StringToLatitude(message[2], message[3]);
                gps.Longitude = StringToLongitude(message[4], message[5]);
                gps.Quality = (FixQuality)int.Parse(message[6], CultureInfo.InvariantCulture);
                gps.NumberOfSatellites = int.Parse(message[7], CultureInfo.InvariantCulture);
                gps.Hdop = StringToDouble(message[8]);
                gps.Altitude = StringToDouble(message[9]);
                gps.AltitudeUnits = message[10];
                gps.GeoidalSeparation = StringToDouble(message[11]);
                gps.GeoidalSeparationUnits = message[12];

                return gps;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Parsing GPS Data: {gpggaLine}", ex);
            }
        }

        internal static double StringToLatitude(string value, string ns)
        {
            if (value == null || value.Length < 3)
                return double.NaN;
            double latitude = int.Parse(value.Substring(0, 2), CultureInfo.InvariantCulture) + double.Parse(value.Substring(2), CultureInfo.InvariantCulture) / 60;
            if (ns == "S")
                latitude *= -1;
            return latitude;
        }

        internal static double StringToLongitude(string value, string ew)
        {
            if (value == null || value.Length < 4)
                return double.NaN;
            double longitude = int.Parse(value.Substring(0, 3), CultureInfo.InvariantCulture) + double.Parse(value.Substring(3), CultureInfo.InvariantCulture) / 60;
            if (ew == "W")
                longitude *= -1;
            return longitude;
        }

        internal static double StringToDouble(string value)
        {
            if (value != null && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return double.NaN;
        }
        internal static TimeSpan StringToTimeSpan(string value)
        {
            if (value != null && value.Length >= 6)
            {
                return new TimeSpan(int.Parse(value.Substring(0, 2), CultureInfo.InvariantCulture),
                                   int.Parse(value.Substring(2, 2), CultureInfo.InvariantCulture), 0)
                                   .Add(TimeSpan.FromSeconds(double.Parse(value.Substring(4), CultureInfo.InvariantCulture)));
            }
            return TimeSpan.Zero;
        }
    }

    public enum FixQuality : int
    {
        Invalid = 0,
        GpsFix = 1,
        DgpsFix = 2,
        PpsFix = 3,
        Rtk = 4,
        FloatRtk = 5,
        Estimated = 6,
        ManualInput = 7,
        Simulation = 8
    }

    public class GGAMessage
    {
        public TimeSpan? TimeStamp;
        public double Latitude;
        public double Longitude;
        public FixQuality Quality;
        public int? NumberOfSatellites;
        public double Hdop;
        public double Altitude;
        public string AltitudeUnits;
        public double GeoidalSeparation;
        public string GeoidalSeparationUnits;
    }
}