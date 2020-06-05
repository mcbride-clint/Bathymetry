using System;
using System.IO.Ports;

namespace Bathymetry.Data.Providers
{
    public class SerialReadingProvider : IReadingProvider, IDisposable
    {
        private readonly SerialPort _serialPort;

        public bool IsStarted {
            get => _serialPort.IsOpen;
            set => throw new NotImplementedException(); 
        }

        public SerialReadingProvider(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public event EventHandler<string> OnReadingRecieved;

        public void Start()
        {
            if (_serialPort.IsOpen)
            {
                return;
            }

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            _serialPort.Open();
        }

        public void Stop()
        {
            _serialPort.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            OnReadingRecieved?.Invoke(null, indata);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
