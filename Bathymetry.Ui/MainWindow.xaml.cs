using Bathymetry.Data;
using Bathymetry.Data.Models;
using Bathymetry.Data.Providers;
using Boyd.NMEA.NMEA.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ReadingParser _parser;
        private IReadingProvider _provider;

        private ConcurrentBag<Reading> readings = new ConcurrentBag<Reading>();

        public MainWindow()
        {
            InitializeComponent();           

            _provider = App.ServiceProvider.GetService<IReadingProvider>();
            _parser = App.ServiceProvider.GetService<ReadingParser>();

            _provider.OnReadingRecieved += DataRecieved;
        }

        private void DataRecieved(object sender, string recievedText)
        {
            var data = _parser.Parse(recievedText);

            if (!data.IsValid)
            {
                return;
            }

            readings.Add(data);

            Dispatcher.Invoke(() =>
            {
                
                OutputStream.Text = data.ToString() + OutputStream.Text;
            });
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_provider.IsStarted)
            {
                _provider.Stop();
            }
            else
            {
                _provider.Start();
            }
        }

        private void Open3dButton_Click(object sender, RoutedEventArgs e)
        {
            WriteOutData();

            if (File.Exists("Html\\Viewer.html"))
            {
                System.Diagnostics.Process.Start("Html\\Viewer.html");
            }
        }

        private void WriteOutData()
        {
            var sb = new StringBuilder();

            foreach(var reading in readings)
            {
                sb.AppendLine($"{reading.Location.Latitude},{reading.Location.Longitude},{reading.Depth.F1},{reading.Depth.F2}");
            }

            File.WriteAllText("Html\\data.csv", sb.ToString());
        }
    }
}
