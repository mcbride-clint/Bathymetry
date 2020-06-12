using Bathymetry.Data.Models;
using System;
using System.Linq;

namespace Bathymetry.Data
{
    public class ReadingParser
    {
        private readonly GpggaNmeaParser _nmeaParser;

        public ReadingParser(GpggaNmeaParser nmeaParser)
        {
            _nmeaParser = nmeaParser;
        }

        public Reading Parse(string inputText)
        {
            var lines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var reading = new Reading();

            if (lines.Count == 2)
            {
                foreach (var line in lines)
                {
                    if (line.StartsWith("$GPGGA"))
                    {
                        reading.Location = (GGAMessage)ParseNmea(line);
                    }
                    else
                    {
                        reading.Depth = ParseDepth(line);
                    }
                }
            }
            else
            {
                // Bad Line
            }

            return reading;
        }

        private DepthInfo ParseDepth(string readingLine)
        {
            var info = new DepthInfo();

            var parts = readingLine.Split(',');

            info.F1 = decimal.Parse(parts[3]);
            info.Unit = parts[4];
            info.F2 = decimal.Parse(parts[8]);

            return info;
        }

        private GGAMessage ParseNmea(string gpsLine)
        {
            return _nmeaParser.Parse(gpsLine);
        }
    }
}
