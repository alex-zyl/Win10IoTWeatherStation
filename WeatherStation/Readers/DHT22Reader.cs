using System;
using System.Threading.Tasks;
using WeatherStation.Readings;
using Windows.Devices.Gpio;
using Sensors.Dht;

namespace WeatherStation.Readers
{
    public class DHT22Reader : ITemperatureReader
    {
        private Dht22 _sensor;

        public bool IsInitialized { get; private set; }

        public Task InitializeAsync()
        {
            var controller = GpioController.GetDefault();

            var dataPin = controller.OpenPin(4, GpioSharingMode.Exclusive);
            var triggerPin = controller.OpenPin(11, GpioSharingMode.Exclusive);
            _sensor = new Dht22(dataPin, GpioPinDriveMode.Input);
            IsInitialized = true;
           // return _sensor.Initialize();
            return Task.CompletedTask;
        }

        public async Task<TemperatureReadings> ReadAsync()
        {

            var reading = await _sensor.GetReadingAsync().AsTask();

            var readings = new TemperatureReadings
            {
                //Temperature = reading.Temperature,
                //Humidity = reading.Humidity
            };

            return readings;
        }

        public TemperatureSensorType Type => TemperatureSensorType.DHT22;
    }
}