using WeatherStation.Readers;
using WeatherStation.Readings;

namespace WeatherStation.Extensions
{
    public static class SensorReaderExtensions
    {
        public static SensorReaderRunner WithTemperature(this SensorReaderRunner readerRunner, TemperatureSensorType type)
        {
            ITemperatureReader reader = null;

            switch (type)
            {
                case TemperatureSensorType.BME280:
                    reader = new BME280Reader();
                    break;
                case TemperatureSensorType.DHT22:
                    reader = new DHT22Reader();
                    break;
            }
            if(reader != null)
                readerRunner.TemperatureReaders.Add(reader);
            return readerRunner;
        }

        public static SensorReaderRunner WithCO2(this SensorReaderRunner readerRunner, CO2SensorType type)
        {
            ICO2Reader reader = null;
            switch (type)
            {
                case CO2SensorType.SenseAirS8:
                    reader = new SenseAirS8Reader();
                    break;
                case CO2SensorType.MH_Z19B:
                    reader = null;
                    break;
            }
            if (reader != null)
                readerRunner.CO2Readers.Add(reader);

            return readerRunner;
        }
    }
}