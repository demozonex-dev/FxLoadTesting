// See https://aka.ms/new-console-template for more information
using Azure.Core;
using fx.Injector;
using Fx.ArmManager;
using Fx.Injector;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

TokenCredential tokenCredential = await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode);
var config = Fx.Helpers.Configuration.Create();
var parameterSection = config.GetSection("parameters");
string? resourceGroupName = parameterSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }


ResourceClient resourceClient = new ResourceClient();
await resourceClient.EasyInitAsync(resourceGroupName, tokenCredential);
string[] arguments = Environment.GetCommandLineArgs();
short maxMessage = 2;

IInjector injector = null;
if (arguments.Length > 1)
{
    switch (arguments[1].ToLower())
    {
        case "eventgrid":
            injector = await CreateEventGridInjector(resourceClient, parameterSection);
            break;
        case "servicebus":            
            injector=await CreateServiceBusInjector(resourceClient, parameterSection);
            break;
        case "webpubsub":
            injector=await CreateWebPubSubInjector(resourceClient, parameterSection);
            break;
        case "storagequeue":
            injector=await CreateStorageQueueInjector(resourceClient, parameterSection);
            break;
        default:
            Console.WriteLine("Unknow injector : eventgrid, servicebus, webpubsub, storagequeue");
            break;
    }
}
else
{
    Console.WriteLine("Unknow injector : eventgrid, servicebus, webpubsub, storagequeue");
};

if (injector != null)
{


    string host=Fx.Helpers.NetworkInfo.GetHostName();
    bool sendAnotherMessage = false;
    do
    {

        
        for (int i = 1; i <= maxMessage; i++)
        {
            Messages.DemoMessage data = new Messages.DemoMessage
            {
                Id = Guid.NewGuid().ToString(),
                Description = "message send",
                InjectorDate = DateTime.Now,
                Injector = injector.InjectorType,
                Host = host,
            };
            string jsonData = JsonSerializer.Serialize(data);
            await injector.SendAsync(jsonData);
            Console.WriteLine($"Message {i} Sent");
        }
        Console.WriteLine("Do you want to send more messages ? (y/n)");
        ConsoleKeyInfo key = Console.ReadKey();
        sendAnotherMessage = (key.KeyChar == 'y' || key.KeyChar == 'Y');
    }
    while (sendAnotherMessage);
}

static async Task<IInjector> CreateStorageQueueInjector(ResourceClient resourceclient, 
                                                  IConfigurationSection parametersection)
{

    Console.WriteLine("Demo sending Message to Storage Queue");
    Console.WriteLine("Enter any key to send the messages");
    Console.ReadLine();
    string? account = parametersection["accountname:value"];
    if (account == null) { throw new NullReferenceException(nameof(account)); }
    string? queueName = parametersection["storagequeue:value"];
    if (queueName == null) { throw new NullReferenceException(nameof(queueName)); }



    string? connectionString = await resourceclient.GetStorageConnectionStringAsync(account);
    if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }
    return new StorageQueue(connectionString, queueName);
}
static async Task<IInjector> CreateWebPubSubInjector(ResourceClient resourceclient, 
                                                     IConfigurationSection parametersection)
{
    Console.WriteLine("Demo sending Message to WebPubSub (WebSocket)");
    Console.WriteLine("Enter any key to send the messages");
    Console.ReadLine();
    string? webPubSub = parametersection["webpubsub:value"];
    if (webPubSub == null) { throw new NullReferenceException(nameof(webPubSub)); }
    string? hubname = parametersection["webpubsubhubname:value"];
    if (hubname == null) { throw new NullReferenceException(nameof(hubname)); }


    
    string? connectionString = await resourceclient.GetWebPubSubConnectionStringAsync(webPubSub);
    if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }

    return new WebPubSub(connectionString, hubname);

}
static async Task<IInjector> CreateServiceBusInjector(ResourceClient resourceclient, IConfigurationSection parametersection)
{
    Console.WriteLine("Demo sending Message to Service Bus");
    Console.WriteLine("Enter any key to send the messages");
    Console.ReadLine();
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


static async Task<IInjector> CreateEventGridInjector(ResourceClient resourceclient, IConfigurationSection parametersection)
{

    Console.WriteLine("Demo sending Messages to Event Grid");
    Console.WriteLine("Enter any key to send the messages");
    Console.ReadLine();

    string? topicName = parametersection["eventgridtopic:value"];
    if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }
    var eventGridConnectionInfos = await resourceclient.GetEventGridConnectionInfosAsync(topicName);    
    return  new Fx.Injector.EventGrid(eventGridConnectionInfos.endpoint, eventGridConnectionInfos.key);



    
    

    
}


