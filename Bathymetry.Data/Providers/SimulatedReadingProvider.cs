using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Bathymetry.Data.Providers
{
    public class SimulatedReadingProvider : IReadingProvider
    {
        public SimulatedReadingProvider(ILogger<SimulatedReadingProvider> logger)
        {
            _logger = logger;
        }

        public event EventHandler<string> OnReadingRecieved;

        private Timer timer;
        private readonly ILogger<SimulatedReadingProvider> _logger;

        public bool IsStarted { get; set; }

        public void Start()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;

            _logger.LogInformation($"Starting Simulated Reader at interval of {timer.Interval}");

            timer.Start();

            IsStarted = true;
        }

        public void Stop()
        {
            timer.Stop();

            IsStarted = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnReadingRecieved?.Invoke(null, GenerateText());
        }

        private string GenerateText()
        {
            var rand = new Random();
            var f1 = Math.Round(rand.NextDouble() + 10, 1);
            var f2 = Math.Round(rand.NextDouble() + 16, 1);

            // TODO Vary Lat and Longitude
            var latitude = Math.Round(rand.NextDouble() * 10 + 4000, 6);
            var longitude = Math.Round(rand.NextDouble() * 10 + 7900, 6);

            return $"$GPGGA,194414.00,{latitude},N,{longitude},W,2,08,1.4,307.71,M,-33.04,M,7.0,0133*78" +
                Environment.NewLine + $"$PIST,DBT,F1,0{f1},f,,M,F2,0{f2},f,,M*63" + Environment.NewLine;
        }
    }
}
