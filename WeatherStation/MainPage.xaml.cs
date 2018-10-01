using System;
using WeatherStation.Extensions;
using WeatherStation.Readings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WeatherStation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StationState state = StationState.NonInitialized;
        private const string deviceId = "weather-rpi-device";
        private SensorReaderRunner _readerRunner;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            state = StationState.Initializing;
            _readerRunner = new SensorReaderRunner(deviceId)
                .WithTemperature(TemperatureSensorType.BME280);
                //.WithTemperature(TemperatureSensorType.DHT22);
            //.WithCO2(CO2SensorType.SenseAirS8);

            _readerRunner.OnReading += async data => { await SensorDataList.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PrintReadings(data)); };
            _readerRunner.OnError += async exception =>
            {
                await SensorDataList.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => SensorDataList.Items.Add($"Exception: Type {exception.GetType()}; Message: {exception.Message}"));
            };

            await _readerRunner.Initialize();
            
            _readerRunner.Start();

            state = StationState.Active;
            ActivityBtn.IsEnabled = true;
        }

        private void InitControls()
        {
            //foreach (var temp in _readerRunner.TemperatureReaders)
            //{
            //    SensorDiplayPanel.Children.Add(new TextBlock
            //    {
            //        Text = $"Temperature {temp.Type}:"
            //    });
            //}
        }

        private void PrintReadings(AggregatedSensorReadings data)
        {
            string line = string.Empty;
            foreach (var temp in data.Temperature)
            {
                line += $"{temp.Source} Sensor: ";
                if (temp.Temperature != null)
                    line += $"Temperature: {temp.Temperature}; ";

                if (temp.Humidity != null)
                    line += $"Humidity: {temp.Humidity} %; ";

                if (temp.Pressure != null)
                    line += $"Humidity: {temp.Pressure} Pa; ";

                SensorDataList.Items.Add(line);
            }

            foreach (var temp in data.CO2)
            {
                line += $"{temp.Source} Sensor: ";

                SensorDataList.Items.Add(line);
            }

            if (SensorDataList.Items.Count > 1000)
                SensorDataList.Items.RemoveAt(0);
        }

        private void ActivityBtn_OnClick(object sender, RoutedEventArgs e)
        {
            switch (state)
            {
                case StationState.Paused:
                    _readerRunner.Start();
                    ActivityBtn.Content = "Start";
                    state = StationState.Active;
                    break;
                case StationState.Active:
                    _readerRunner.Pause();
                    ActivityBtn.Content = "Pause";
                    state = StationState.Paused;
                    break;
            }
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            SensorDataList.Items.Clear();
        }
    }
}