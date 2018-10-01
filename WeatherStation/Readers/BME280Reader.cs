using System.Threading.Tasks;
using BME280;
using WeatherStation.Extensions;
using WeatherStation.Readings;

namespace WeatherStation.Readers
{
    public class BME280Reader : ITemperatureReader
    {
        private const byte BME280_I2C_ADDRESS = 0x76;
        private BME280Sensor _bme280;

        public bool IsInitialized { get; private set; }

        public async Task InitializeAsync()
        {
            _bme280 = new BME280Sensor(BME280_I2C_ADDRESS);
            await _bme280.Initialize();
            IsInitialized = true;
        }

        public async Task<TemperatureReadings> ReadAsync()
        {
            float temp = await _bme280.ReadTemperature();
            float humidity = await _bme280.ReadHumidity();
            float pressure = await _bme280.ReadPressure();

            var data = new TemperatureReadings
            {
                Temperature = temp.Truncate(2),
                Humidity = humidity.Truncate(2),
                Pressure = pressure.Truncate(2),
                Source = TemperatureSensorType.BME280
            };
            return data;
        }
    }
}