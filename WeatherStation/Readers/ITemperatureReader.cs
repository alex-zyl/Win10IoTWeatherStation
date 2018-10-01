using WeatherStation.Readings;

namespace WeatherStation.Readers
{
    public interface ITemperatureReader : ISensorReader<TemperatureReadings>
    {
    }
}