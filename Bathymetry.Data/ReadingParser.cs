using Bathymetry.Data.Models;
using DotnetNMEA.NMEA0183;
using DotnetNMEA.NMEA0183.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bathymetry.Data
{
    public class ReadingParser
    {
        private readonly NMEA0183Parser _nmeaParser;

        public ReadingParser(NMEA0183Parser nmeaParser)
        {
            _nmeaParser = nmeaParser;
            HasGps = true;
            HasF1 = true;
            HasF2 = true;
        }

        public bool HasGps { get; set; }
        public bool HasF1 { get; set; }
        public bool HasF2 { get; set; }
        public Reading Parse(string inputText)
        {
            var lines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var reading = new Reading();

            if (HasGps)
            {
                reading.Location = (GGAMessage)ParseGps(lines[0]);
                reading.Depth = ParseReading(lines[1]);
            } 
            else
            {
                reading.Depth = ParseReading(lines[0]);
            }

            return reading;
        }

        private DepthInfo ParseReading(string readingLine)
        {
            var info = new DepthInfo();
           
            var parts = readingLine.Split(',');

            info.F1 = decimal.Parse(parts[3]);
            info.Unit = parts[4];
            info.F2 = decimal.Parse(parts[8]);

            return info;
        }

        private Nmea0183Message ParseGps(string gpsLine)
        {
            return _nmeaParser.Parse(gpsLine.AsSpan());
        }
    }
}
