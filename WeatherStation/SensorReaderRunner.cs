using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherStation.Readers;
using WeatherStation.Readings;
using WeatherStation.Sender;

namespace WeatherStation
{
    public class SensorReaderRunner : IDisposable
    {
        private Task runner;
        private CancellationTokenSource tokenSource;
        private readonly SensorReadingsSender readingsSender;
        private ManualResetEventSlim eventLock = new ManualResetEventSlim(false);
        private readonly List<ITemperatureReader> temperatureReaders = new List<ITemperatureReader>();
        private readonly List<ICO2Reader> co2Readers = new List<ICO2Reader>();

        public event Action<AggregatedSensorReadings> OnReading;
        public event Action<Exception> OnError;
        public ICollection<ITemperatureReader> TemperatureReaders => temperatureReaders;
        public ICollection<ICO2Reader> CO2Readers => co2Readers;

        public SensorReaderRunner(string deviceId)
        {
            readingsSender = new SensorReadingsSender(deviceId);

            OnReading += reading => { };
            OnError += exception => { };
        }

        public async Task Initialize()
        {
            tokenSource = new CancellationTokenSource();

            await InitializeReaders();

            runner = Task.Factory.StartNew(async () =>
            {
                do
                {
                    try
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();
                        if (!eventLock.IsSet)
                            eventLock.Wait(tokenSource.Token);

                        var data = await ReadAsync();
                        OnReading(data);
                        await readingsSender.SendAsync(data);
                        await Task.Delay(TimeSpan.FromSeconds(5), tokenSource.Token);
                    }
                    catch (Exception ex) when (ex is TaskCanceledException == false)
                    {
                        OnError(ex);
                    }
                } while (true);
            }, tokenSource.Token);
        }

        private async Task InitializeReaders()
        {
            var initializers = temperatureReaders.Select(x => x.InitializeAsync())
                .Union(co2Readers.Select(x => x.InitializeAsync())).ToArray();

            await Task.WhenAll(initializers);

            foreach (var task in initializers)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }
            }
        }

        public void Start()
        {
            eventLock.Set();
        }

        public void Pause()
        {
            eventLock.Reset();
        }

        private async Task<AggregatedSensorReadings> ReadAsync()
        {
            var data = new AggregatedSensorReadings();

            await ReadTemperature(data);
            await ReadCO2(data);

            data.ReadingTime = DateTimeOffset.UtcNow;
            return data;
        }

        private async Task ReadCO2(AggregatedSensorReadings data)
        {
            foreach (var reader in co2Readers)
            {
                if (!reader.IsInitialized)
                    continue;

                var readings = await reader.ReadAsync();
                if (readings != null)
                    data.CO2.Add(readings);
            }
        }

        private async Task ReadTemperature(AggregatedSensorReadings data)
        {
            foreach (var reader in temperatureReaders)
            {
                if (!reader.IsInitialized)
                    continue;

                var readings = await reader.ReadAsync();
                if (readings != null)
                    data.Temperature.Add(readings);
            }
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            readingsSender?.Dispose();
            eventLock?.Dispose();
        }
    }
}