using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using NithinsPI.Common;

namespace NithinsPI.Device
{
    class Program
    {
        private const string DeviceConnectionString = "HostName=NithinsPI.azure-devices.net;DeviceId=NithinsPI;SharedAccessKey=3/Di9ndJYFzf5JphzW8YzRM7HebOhKoUGoqmK6Xh/cY=";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello From Nithin's PI!");

            var device = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

            await device.OpenAsync();

            var receiveEvents = ReceiveEvents(device);

            Console.WriteLine("Device is connected!");

            await UpdateTwin(device);

            Console.WriteLine("Press a key to perform an action:");
            Console.WriteLine("q: quits");
            Console.WriteLine("h: send happy feedback");
            Console.WriteLine("u: send unhappy feedback");
            Console.WriteLine("e: request emergency help");

            var random = new Random();
            var quitRequested = false;
            while (!quitRequested)
            {
                Console.Write("Action? ");
                var input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                var status = StatusType.NotSpecified;
                var latitude = random.Next(0, 100);
                var longitude = random.Next(0, 100);

                switch (Char.ToLower(input))
                {
                    case 'q':
                        quitRequested = true;
                        break;
                    case 'h':
                        status = StatusType.Happy;
                        break;
                    case 'u':
                        status = StatusType.UnHappy;
                        break;
                    case 'e':
                        status = StatusType.Emergency;
                        break;
                    default:
                        break;
                }

                var telemetry = new Telemetry
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Status = status
                };

                var payLoad = JsonConvert.SerializeObject(telemetry);
                var message = new Message(Encoding.ASCII.GetBytes(payLoad));

                await device.SendEventAsync(message);

                Console.WriteLine("Messge Sent!");
            }
        }

        private static async Task ReceiveEvents(DeviceClient device)
        {
            while (true)
            {
                var message = await device.ReceiveAsync();
                if (message == null)
                {
                    continue;
                }
                var messageBody = message.GetBytes();

                var payLoad = Encoding.ASCII.GetString(messageBody);

                Console.WriteLine($"Received message from cloud: '{payLoad}'");
                await device.CompleteAsync(message);

            }
        }

        private static async Task UpdateTwin(DeviceClient device)
        {
            var twinProperties = new TwinCollection();

            twinProperties["ConnectionType"] = "wi-fi";
            twinProperties["connectionStrength"] = "full";

            await device.UpdateReportedPropertiesAsync(twinProperties);
        }
    }
}
