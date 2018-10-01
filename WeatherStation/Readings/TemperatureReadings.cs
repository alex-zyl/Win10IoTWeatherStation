namespace WeatherStation.Readings
{
    public class TemperatureReadings
    {
        public TemperatureSensorType Source { get; set; }
        public float? Humidity;
        public float? Temperature;
        public float? Pressure;
    }
}