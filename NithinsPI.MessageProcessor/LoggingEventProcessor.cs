using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using NithinsPI.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NithinsPI.MessageProcessor
{
    internal class LoggingEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("LoggingEventProcessor closing, processing partition: " + $"'{context.PartitionId}', reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("LoggingEventProcessor opened, processing partition: " + $"'{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine("LoggingEventProcessor error, processing partition: " + $"'{context.PartitionId}, error:{error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var payload = Encoding.ASCII.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var deviceId = eventData.SystemProperties["iothub-connection-device-id"];


                Console.WriteLine($"Message received on partition '{context.PartitionId}', " + $"device ID: '{deviceId}', " + $"payload: '{payload}'");

                var telemetry = JsonConvert.DeserializeObject<Telemetry>(payload);

                if (telemetry.Status == StatusType.Emergency)
                {
                    Console.WriteLine($"Guest requires emergency assistance! Device ID: {deviceId}");
                    SendFirstRespondersTo(telemetry.Latitude, telemetry.Longitude);
                }
            }
            return context.CheckpointAsync();
        }

        private void SendFirstRespondersTo(decimal latitude, decimal longitude)
        {
            Console.WriteLine($"First responders dispatched to {latitude}, {longitude} !");
        }
    }
}