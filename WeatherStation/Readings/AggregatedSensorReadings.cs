using System;
using System.Collections.Generic;

namespace WeatherStation.Readings
{
    public class AggregatedSensorReadings
    {
        public AggregatedSensorReadings()
        {
            CO2 = new List<CO2Readings>();
            Temperature = new List<TemperatureReadings>();
        }

        public IList<CO2Readings> CO2 { get; set; }
        public IList<TemperatureReadings> Temperature { get; set; }
        public DateTimeOffset ReadingTime { get; set; }
    }
}