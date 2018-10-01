using System.Threading.Tasks;

namespace WeatherStation.Readers
{
    public interface ISensorReader<TReadings>
    {
        bool IsInitialized { get; }
        Task InitializeAsync();
        Task<TReadings> ReadAsync();
    }
}