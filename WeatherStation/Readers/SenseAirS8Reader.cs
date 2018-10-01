using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using WeatherStation.Readings;

namespace WeatherStation.Readers
{
    public class SenseAirS8Reader : ICO2Reader
    {
        public bool IsInitialized { get; private set; }

        public Task InitializeAsync()
        {
            IsInitialized = false;
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }

        public async Task<CO2Readings> ReadAsync()
        {
            return null;
            //GpioController gpio = GpioController.GetDefault();

            //// Open GPIO 5
            //using (GpioPin pin = gpio.OpenPin(5))
            //{
            //    // Latch HIGH value first. This ensures a default value when the pin is set as output
            //    pin.Write(GpioPinValue.High);

            //    // Set the IO direction as output
            //    pin.SetDriveMode(GpioPinDriveMode.Output);

            //} // Close pin - will revert to its power-on state

            string aqs = SerialDevice.GetDeviceSelector();

            var dis = await DeviceInformation.FindAllAsync(aqs);

            SerialDevice serialPort = await SerialDevice.FromIdAsync(dis[0].Id);

            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = 9600;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = SerialHandshake.None;

            var dataReaderObject = new DataReader(serialPort.InputStream);

            uint ReadBufferLength = 1024;

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            UInt32 bytesRead = await dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

            if (bytesRead > 0)
            {
                var dataString = dataReaderObject.ReadString(bytesRead);
            }

            //throw new System.NotImplementedException();
        }
    }
}