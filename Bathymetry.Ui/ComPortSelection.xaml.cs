using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using System.Windows;

namespace Bathymetry.Ui
{
    public partial class Com_Port_Selection : Window
    {
        public List<string> AvailablePortNames { get; set; }
        public string MatchedPortName { get; set; }

        private int AttemptCount { get; set; } = 1;

        public Com_Port_Selection()
        {
            InitializeComponent();

            SearchForPort();
        }

        private void SearchForPort()
        {
            UpdateStatusLabel("Searching...");

            AvailablePortNames = SerialPort.GetPortNames().ToList();

            foreach (var portName in AvailablePortNames)
            {
                UpdateStatusLabel(portName);
                if (CheckComPortForData("Searching: " + portName))
                {
                    UpdateStatusLabel($"Found Bathymetry System on {portName}");
                    MatchedPortName = portName;
                    AddSerialPortButton.IsEnabled = true;
                    return;
                }
            }


            // No applicable port found.  Search Again
            UpdateStatusLabel($"No Valid Serial Port Found. \r\n Searching again in 5 seconds \r\n Attempt: {AttemptCount}");

            var SearchTimer = new Timer();
            SearchTimer.Interval = 5000;
            SearchTimer.AutoReset = false;
            SearchTimer.Elapsed += SearchTimer_Elapsed;

            SearchTimer.Start();
        }

        private void SearchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AttemptCount++;
            SearchForPort();
        }

        private void UpdateStatusLabel(string text)
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Text = text;
            });
        }

        private void UpdateDataExampleLabel(string text)
        {
            Dispatcher.Invoke(() =>
            {
                DataExampleLabel.Text = text;
            });
        }

        private bool CheckComPortForData(string portName)
        {
            SerialPort serialPort = null;
            try
            {
                serialPort = App.CreateSerialPort(portName);
                serialPort.Open();
                var textLine = serialPort.ReadLine();

                if (textLine == "$GPGGA" || textLine.StartsWith("$PIST"))
                {
                    UpdateDataExampleLabel(textLine);
                    return true;
                }
            }
            catch
            {
                // Something Wrong with this port.
            }
            finally
            {
                if (serialPort != null && serialPort.IsOpen)
                    serialPort.Close();
            }

            return false;
        }

        private void AddSerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            App.AddComPort(MatchedPortName);
            ShowMainWindow();
        }

        private void AddSimulatedButton_Click(object sender, RoutedEventArgs e)
        {
            App.AddSimulatedComPort();
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            App.ServiceProvider = App.ServiceCollection.BuildServiceProvider();

            var newWindow = new MainWindow();
            newWindow.Show();

            Close();
        }
    }
}
