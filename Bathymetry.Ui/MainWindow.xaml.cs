using Bathymetry.Data;
using Bathymetry.Data.Models;
using Bathymetry.Data.Providers;
using DotnetNMEA.NMEA0183;
using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ReadingParser _parser;
        private IReadingProvider _provider;
        public ObservableCollection<ChartViewModel> F1Values = new ObservableCollection<ChartViewModel>();



        public MainWindow()
        {
            InitializeComponent();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(/*logging => logging.AddConsole()*/)
                //.AddSingleton<SerialPort>(CreateSerialPort("COM4"))
                //.AddSingleton<IReadingProvider, SerialReadingProvider>()
                .AddSingleton<NMEA0183Parser>()
                .AddSingleton<ReadingParser>()
                .AddSingleton<IReadingProvider, SimulatedReadingProvider>()
                .BuildServiceProvider();

            //do the actual work here
            _provider = serviceProvider.GetService<IReadingProvider>();
            _parser = serviceProvider.GetService<ReadingParser>();

            _provider.OnReadingRecieved += DataRecieved;

            Series.DataContext = F1Values;
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

            var f1Point = new ChartViewModel();
            f1Point.Longitude = data.Location.Longitude;
            f1Point.Latitude = data.Location.Latitude;
           
            Dispatcher.Invoke(() =>
            {
                F1Values.Add(f1Point);
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
    }
}
