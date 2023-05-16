// See https://aka.ms/new-console-template for more information
using Azure.Core;
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
    switch (arguments[1])
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
        default:
            Console.WriteLine("Unknow injector : eventgrid, servicebus");
            break;
    }
}
else
{
    Console.WriteLine("missing receiver : eventgrid, servicebus");    
};

if (injector != null)
{


string host=Fx.Helpers.NetworkInfo.GetHostName();

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


    //TODO Get Connection string via code
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


