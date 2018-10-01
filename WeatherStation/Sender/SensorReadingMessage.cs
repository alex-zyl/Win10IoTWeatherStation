using System;
using Newtonsoft.Json;

namespace WeatherStation.Sender
{
    internal class SensorReadingMessage
    {
        public SensorReadingMessage(string deviceId)
        {
            DeviceId = deviceId;
        }

        [JsonProperty(PropertyName = "id")]
        public string DeviceId;
        [JsonProperty(PropertyName = "rt")]
        public DateTimeOffset ReadingTime;
        [JsonProperty(PropertyName = "h")]
        public float Humidity;
        [JsonProperty(PropertyName = "t")]
        public float Temperature;
        [JsonProperty(PropertyName = "p")]
        public float Pressure;
        [JsonProperty(PropertyName = "co")]
        public float CO2;
    }
}