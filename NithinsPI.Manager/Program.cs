using Microsoft.Azure.Devices;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NithinsPI.Manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceConnectionString = "HostName=NithinsPI.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=cp1PVyzq2KSUclksBQcCzsRJmNcJn+ALJx8NNQgipbk=";

            var serviceClient = ServiceClient.CreateFromConnectionString(serviceConnectionString);
            var feedbackTask = ReceiveFeedback(serviceClient);

            while (true)
            {
                Console.WriteLine("Which device do you want to send a message to? ");
                Console.WriteLine("> ");

                var deviceId = Console.ReadLine();

                await SendCloudToDeviceMessage(serviceClient, deviceId);

            }
        }

        private static async Task ReceiveFeedback(ServiceClient serviceClient)
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();

                if (feedbackBatch == null)
                {
                    continue;
                }

                foreach (var record in feedbackBatch.Records)
                {
                    var messageId = record.OriginalMessageId;
                    var status = record.StatusCode;

                    Console.WriteLine($"Feedback for message '{messageId}', status code: '{status}'");
                }

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }

        private static async Task SendCloudToDeviceMessage(ServiceClient serviceClient, string deviceId)
        {
            Console.WriteLine("What message payload do you want to send? ");
            Console.Write("> ");

            var payload = Console.ReadLine();

            var commandMessage = new Message(Encoding.ASCII.GetBytes(payload));
            commandMessage.MessageId = Guid.NewGuid().ToString();
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            commandMessage.ExpiryTimeUtc = DateTime.UtcNow.AddSeconds(10);

            await serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}
