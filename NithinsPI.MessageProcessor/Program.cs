using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace NithinsPI.MessageProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hubName = "";
            var iotHubConnectionString = "";
            var storageConnectionString = "";
            var storageContainerName = "message-processor-host";
            var consumerGroupName = PartitionReceiver.DefaultConsumerGroupName;

            var processor = new EventProcessorHost(
                hubName,
                consumerGroupName,
                iotHubConnectionString,
                storageConnectionString,
                storageContainerName
                );

            await processor.RegisterEventProcessorAsync<LoggingEventProcessor>();

            Console.WriteLine("Event Processor started, press enter to exit");

            Console.ReadLine();

            await processor.UnregisterEventProcessorAsync();


        }
    }
}
