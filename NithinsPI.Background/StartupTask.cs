using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using GrovePi;
using GrovePi.Sensors;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NithinsPI.Background
{
    public sealed class StartupTask : IBackgroundTask
    {

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Application is running!");


            var DeviceConnectionString = "NithinsPI.azure-devices.net";
            var DeviceId = "NithinsPI";
            var DeviceKey = "3/Di9ndJYFzf5JphzW8YzRM7HebOhKoUGoqmK6Xh/cY=";

            var device = DeviceClient.Create(DeviceConnectionString, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey),
                TransportType.Http1);

            IDHTTemperatureAndHumiditySensor sensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin4, DHTModel.Dht11);
            ILed greenLed = DeviceFactory.Build.Led(Pin.DigitalPin5);

            while (true)
            {
                BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

                greenLed.AnalogWrite(Convert.ToByte(255));
                sensor.Measure();
                string sensortemp = sensor.TemperatureInCelsius.ToString();
                string sensorhum = sensor.Humidity.ToString();

                var telemetry = new Telemetry
                {
                    Temperature = sensortemp,
                    Humidity = sensorhum
                };

                var payLoad = JsonConvert.SerializeObject(telemetry);
                var message = new Message(Encoding.ASCII.GetBytes(payLoad));

                await device.SendEventAsync(message).ConfigureAwait(false);
                greenLed.AnalogWrite(Convert.ToByte(0));
                await Task.Delay(TimeSpan.FromHours(1));



            }
        }

        private async Task SyncToAzure()
        {
            var DeviceConnectionString = "NithinsPI.azure-devices.net";
            var DeviceId = "NithinsPI";
            var DeviceKey = "3/Di9ndJYFzf5JphzW8YzRM7HebOhKoUGoqmK6Xh/cY=";

            var device = DeviceClient.Create(DeviceConnectionString, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey),
                TransportType.Amqp);

            IDHTTemperatureAndHumiditySensor sensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin4, DHTModel.Dht11);

            while (true)
            {
                sensor.Measure();
                string sensortemp = sensor.TemperatureInCelsius.ToString();
                string sensorhum = sensor.Humidity.ToString();

                var telemetry = new Telemetry
                {
                    Temperature = sensortemp,
                    Humidity = sensorhum
                };

                var payLoad = JsonConvert.SerializeObject(telemetry);
                var message = new Message(Encoding.ASCII.GetBytes(payLoad));

                await device.SendEventAsync(message);

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
