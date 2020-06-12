using Bathymetry.Data;
using Bathymetry.Data.Models;
using Bathymetry.Data.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Reading CurrentReading { get; set; }

        private static ReadingParser _parser;
        private IReadingProvider _provider;

        private ObservableCollection<Reading> readings = new ObservableCollection<Reading>();
        private List<Reading> LastReadings = new List<Reading>();

        private int TotalRecordCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            _provider = App.ServiceProvider.GetService<IReadingProvider>();
            _parser = App.ServiceProvider.GetService<ReadingParser>();

            ReadingListBox.ItemsSource = readings;
            ScatterPlot.ItemsSource = readings;

            F1Plot.ItemsSource = LastReadings;
            F2Plot.ItemsSource = LastReadings;

            _provider.OnReadingRecieved += DataRecieved;

            ChooseFileLocation();
        }

        private void ChooseFileLocation()
        {
            if (!SaveAs())
                ChooseFileLocation();
        }

        private void DataRecieved(object sender, string recievedText)
        {
            var data = _parser.Parse(recievedText);

            if (data.IsValid)
                Dispatcher.InvokeAsync(() => UpdateUIWithReading(data));
        }

        private void UpdateUIWithReading(Reading data)
        {
            try
            {
                TotalRecordCount++;

                data.RecordNumber = TotalRecordCount;
                readings.Add(data);

                if (TotalRecordCount % int.Parse(AutosaveCount.Text) == 0)
                {
                    Task.Run(() => SaveDataAsync(readings.ToList()));
                }

                LastReadings = readings.OrderByDescending(r => r.RecordNumber).Take(25).ToList();

                F1Plot.ItemsSource = LastReadings;
                F2Plot.ItemsSource = LastReadings;

                if (AutoScroll.IsChecked.GetValueOrDefault())
                {
                    ReadingListBox.ScrollIntoView(data);
                }

                LastF1.Text = data.Depth.F1.ToString();
                LastF2.Text = data.Depth.F2.ToString();
            }
            catch (Exception ex)
            {
                var x = 0;
            }
        }

        private void SaveDataAsync(List<Reading> readingsToSave)
        {
            var sb = new StringBuilder();

            // Add Headers
            sb.AppendLine("Record,Latitude,Units,Longitude,Units,Satellites,F1,F2,Units,Timestamp");

            foreach (var reading in readingsToSave)
            {
                sb.AppendLine(reading.ToCsv());
            }

            File.WriteAllText(SaveLocation, sb.ToString());

            Save3DFile();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_provider.IsStarted)
            {
                StartStopText.Text = "Continue Recording";
                _provider.Stop();
            }
            else
            {
                StartStopText.Text = "Pause Recording";
                _provider.Start();
            }
        }

        private void Open3dButton_Click(object sender, RoutedEventArgs e)
        {
            Save3DFile();

            if (File.Exists("Html\\Viewer.html"))
            {
                System.Diagnostics.Process.Start("Html\\Viewer.html");
            }
        }

        private void Save3DFile()
        {
            var sb = new StringBuilder();

            foreach (var reading in readings)
            {
                sb.AppendLine($"{reading.Location.Latitude},{reading.Location.Longitude},{reading.Depth.F1},{reading.Depth.F2}");
            }

            File.WriteAllText(Save3DFileLocation, sb.ToString());
        }

        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => SaveDataAsync(readings.ToList()));
        }

        private void AutosaveCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (SaveAs())
                Task.Run(() => SaveDataAsync(readings.ToList()));
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmLeave())
            {
                var newWindow = new MainWindow();
                newWindow.Show();

                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmLeave())
            {
                Close();
            }
        }

        private bool ConfirmLeave()
        {
            var message = "Are you sure? \r\n All Unsaved Data will be lost.";
            var title = "Leave Recording Session Confirmation";
            var messageBoxResult = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return messageBoxResult == MessageBoxResult.Yes;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SaveLocation))
                if (!SaveAs())
                {
                    return;
                }

            Task.Run(() => SaveDataAsync(readings.ToList()));
        }

        private string SaveLocation;
        private string Save3DFileLocation;

        private bool SaveAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".csv",
                Filter = "Comma Separated Values | *.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveLocation = saveFileDialog.FileName;
                var dir = Path.GetDirectoryName(SaveLocation);
                var fileName = Path.GetFileNameWithoutExtension(SaveLocation);

                Save3DFileLocation = Path.Combine(dir, fileName + "3D.csv");

                return true;
            }

            return false;
        }
    }
}
