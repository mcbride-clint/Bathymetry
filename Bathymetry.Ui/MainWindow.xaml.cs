using Bathymetry.Data;
using Bathymetry.Data.Models;
using Bathymetry.Data.Providers;
using Boyd.NMEA.NMEA.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            //ReadingListBox.DataContext = readings;

            _provider.OnReadingRecieved += DataRecieved;
        }

        private void DataRecieved(object sender, string recievedText)
        {
            var data = _parser.Parse(recievedText);

            if (!data.IsValid)
            {
                return;
            }

            Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    TotalRecordCount++;

                    data.RecordNumber = TotalRecordCount;
                    readings.Add(data);

                    if(TotalRecordCount % int.Parse(AutosaveCount.Text) == 0)
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
                }
                catch(Exception ex)
                {
                    var x = 0;
                }
            });
        }

        private void SaveDataAsync(List<Reading> readingsToSave)
        {
            var sb = new StringBuilder();

            // Add Headers
            sb.AppendLine("Record,Latitude,Units,NS,Longitude,Units,EW,Satellites,F1,F2,Units,Timestamp");

            foreach (var reading in readingsToSave)
            {
                sb.AppendLine(reading.ToCsv());
            }

            File.WriteAllText("Data.csv", sb.ToString());
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
            WriteOutData();

            if (File.Exists("Html\\Viewer.html"))
            {
                System.Diagnostics.Process.Start("Html\\Viewer.html");
            }
        }

        private void WriteOutData()
        {
            var sb = new StringBuilder();

            foreach (var reading in readings)
            {
                sb.AppendLine($"{reading.Location.Latitude},{reading.Location.Longitude},{reading.Depth.F1},{reading.Depth.F2}");
            }

            File.WriteAllText("Html\\data.csv", sb.ToString());
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
    }
}
