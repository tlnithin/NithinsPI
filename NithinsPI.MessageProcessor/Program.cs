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
            var hubName = "iothub-ehub-nithinspi-216441-42fc0eed8c";
            var iotHubConnectionString = "Endpoint=sb://ihsuprodsgres005dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=h1yRfdggbxC/S0MJePRNo0Syns9GTbQ6tYSLk0H4LDk=";
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=nithinspistorage01;AccountKey=zv+WLvLPwH09LwlYxTvx6/F/Cxy4s8Yi0Hhu3Jw1oKBla88VqGSYTtVfEDkgEQ2xHveqDNSdW19xDkNrAG/jNA==;EndpointSuffix=core.windows.net";
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
