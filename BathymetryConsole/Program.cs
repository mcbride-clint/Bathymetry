using Bathymetry.Data;
using Bathymetry.Data.Providers;
using DotnetNMEA.NMEA0183;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO.Ports;

namespace BathymetryConsole
{
    class Program
    {
        private static ReadingParser _parser;

        static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                })
                //.AddSingleton<SerialPort>(CreateSerialPort("COM4"))
                //.AddSingleton<IReadingProvider, SerialReadingProvider>()
                .AddSingleton<NMEA0183Parser>()
                .AddSingleton<ReadingParser>()
                .AddSingleton<IReadingProvider, SimulatedReadingProvider>()
                .BuildServiceProvider();

            //do the actual work here
            var provider = serviceProvider.GetService<IReadingProvider>();
            _parser = serviceProvider.GetService<ReadingParser>();

            provider.OnReadingRecieved += DataRecieved;
            provider.Start();

            Console.ReadKey();

            provider.Stop();
        }

        private static SerialPort CreateSerialPort(string comPort)
        {
            return new SerialPort(comPort)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8
            };
        }

        private static void DataRecieved(object sender, string recievedText)
        {
            var data = _parser.Parse(recievedText);

            Console.Write(data.ToString());
        }
    }
}
