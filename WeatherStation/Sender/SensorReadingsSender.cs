using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using WeatherStation.Readings;

namespace WeatherStation.Sender
{
    public class SensorReadingsSender : IDisposable
    {
        private const string connectionString = "";
        private readonly DeviceClient deviceClient;

        public SensorReadingsSender(string deviceId)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, deviceId, TransportType.Amqp_WebSocket_Only);
        }

        public Task SendAsync(AggregatedSensorReadings readings)
        {
            var message = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(readings)));

            return deviceClient.SendEventAsync(message);
        }

        public void Dispose()
        {
            deviceClient?.Dispose();
        }
    }
}