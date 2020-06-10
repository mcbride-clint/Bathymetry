using Bathymetry.Data;
using Bathymetry.Data.Models;
using Bathymetry.Data.Providers;
using DotnetNMEA.NMEA0183;
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

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(/*logging => logging.AddConsole()*/)
                //.AddSingleton<SerialPort>(CreateSerialPort("COM6"))
                //.AddSingleton<IReadingProvider, SerialReadingProvider>()
                .AddSingleton<NMEA0183Parser>()
                .AddSingleton<ReadingParser>()
                .AddSingleton<IReadingProvider, SimulatedReadingProvider>()
                .BuildServiceProvider();

            //do the actual work here
            _provider = serviceProvider.GetService<IReadingProvider>();
            _parser = serviceProvider.GetService<ReadingParser>();

            _provider.OnReadingRecieved += DataRecieved;

        }

        private SerialPort CreateSerialPort(string comPort)
        {
            return new SerialPort(comPort)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8
            };
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

            System.Diagnostics.Process.Start("Html\\Viewer.html");
        }

        private void WriteOutData()
        {
            var sb = new StringBuilder();

            //sb.AppendLine("latitude,latitude,f1,f2");

            foreach(var reading in readings)
            {
                sb.AppendLine($"{reading.Location.Latitude},{reading.Location.Longitude},{reading.Depth.F1},{reading.Depth.F2}");
            }

            File.WriteAllText("Html\\data.csv", sb.ToString());
        }
    }
}
