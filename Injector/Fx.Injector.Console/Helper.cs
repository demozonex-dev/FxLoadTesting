using fx.Injector;
using Fx.ArmManager;
using Microsoft.Extensions.Configuration;

namespace Fx.Injector
{
    internal static class Helper
    {

        internal static async Task<IInjector> CreateStorageQueueInjector(ResourceClient resourceclient,
                                                          IConfigurationSection parametersection)
        {

            System.Console.WriteLine("Demo sending Message to Storage Queue");
            System.Console.WriteLine("Enter any key to send the messages");
            System.Console.ReadLine();
            string? account = parametersection["accountname:value"];
            if (account == null) { throw new NullReferenceException(nameof(account)); }
            string? queueName = parametersection["storagequeue:value"];
            if (queueName == null) { throw new NullReferenceException(nameof(queueName)); }



            string? connectionString = await resourceclient.GetStorageConnectionStringAsync(account);
            if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }
            return new StorageQueue(connectionString, queueName);
        }
        internal static async Task<IInjector> CreateWebPubSubInjector(ResourceClient resourceclient,
                                                             IConfigurationSection parametersection)
        {
            System.Console.WriteLine("Demo sending Message to WebPubSub (WebSocket)");
            System.Console.WriteLine("Enter any key to send the messages");
            System.Console.ReadLine();
            string? webPubSub = parametersection["webpubsub:value"];
            if (webPubSub == null) { throw new NullReferenceException(nameof(webPubSub)); }
            string? hubname = parametersection["webpubsubhubname:value"];
            if (hubname == null) { throw new NullReferenceException(nameof(hubname)); }



            string? connectionString = await resourceclient.GetWebPubSubConnectionStringAsync(webPubSub);
            if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }

            return new WebPubSub(connectionString, hubname);

        }
        internal static async Task<IInjector> CreateServiceBusInjector(ResourceClient resourceclient, IConfigurationSection parametersection)
        {
            System.Console.WriteLine("Demo sending Message to Service Bus");
            System.Console.WriteLine("Enter any key to send the messages");
            System.Console.ReadLine();
            string? serviceBus = parametersection["servicebus:value"];
            if (serviceBus == null) { throw new NullReferenceException(nameof(serviceBus)); }
            string? serviceBusQueue = parametersection["servicebusqueue:value"];
            if (serviceBusQueue == null) { throw new NullReferenceException(nameof(serviceBusQueue)); }

            //Get Key
            string? sasKeyName = parametersection["saskeyname:value"];
            if (sasKeyName == null) { throw new NullReferenceException(nameof(sasKeyName)); }

            string? connectionString = await resourceclient.GetServiceBusConnectionStringAsync(serviceBus, sasKeyName);

            return new ServiceBus(connectionString, serviceBusQueue);

        }


        internal static async Task<IInjector> CreateEventGridInjector(ResourceClient resourceclient, IConfigurationSection parametersection)
        {

            System.Console.WriteLine("Demo sending Messages to Event Grid");
            System.Console.WriteLine("Enter any key to send the messages");
            System.Console.ReadLine();

            string? topicName = parametersection["eventgridtopic:value"];
            if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }
            var eventGridConnectionInfos = await resourceclient.GetEventGridConnectionInfosAsync(topicName);
            return new Fx.Injector.EventGrid(eventGridConnectionInfos.endpoint, eventGridConnectionInfos.key);

          }

    }
}
