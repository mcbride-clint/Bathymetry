using Bathymetry.Data;
using Bathymetry.Data.Providers;
using Boyd.NMEA.NMEA;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Ports;
using System.Windows;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; set; }
        public static IServiceCollection ServiceCollection { get; set; }

        public App()
        {
            //setup our DI
            ServiceCollection = new ServiceCollection()
                .AddLogging()
                .AddSingleton<NMEA0183Parser>()
                .AddSingleton<ReadingParser>();
        }

        public static void AddSimulatedComPort()
        {
            ServiceCollection.AddSingleton<IReadingProvider, SimulatedReadingProvider>();
        }

        public static void AddComPort(string portName)
        {
            ServiceCollection.AddSingleton(CreateSerialPort(portName));
            ServiceCollection.AddSingleton<IReadingProvider, SerialReadingProvider>();
        }

        public static SerialPort CreateSerialPort(string comPort)
        {
            return new SerialPort(comPort)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8
            };
        }
    }
}
