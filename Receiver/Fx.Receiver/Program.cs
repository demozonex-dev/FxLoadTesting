// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Fx.ArmManager;
using Fx.Receiver;
using Microsoft.Extensions.Configuration;


Action<string> Wait = (message) =>
{
    Console.WriteLine(message);
    Console.WriteLine("Enter any key to stop");
    //Need here a blocking call 
    //Task.Delay(TimeSpan.FromDays(5)).Wait();
    Console.ReadLine();

};
Action<string> ReturnMessage = (message) =>
{
    
    Console.WriteLine(message);
};

//Create the App Configuration builder in order to get the parameters
var config = Fx.Helpers.Configuration.Create();
var parametersSection = config.GetSection("parameters");
string? resourceGroupName = parametersSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

//Authentication to Azure with device code
TokenCredential credential = 
    await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode);
ResourceClient resourceClient = new ResourceClient();
await resourceClient.EasyInitAsync(resourceGroupName, credential);

Console.ForegroundColor = ConsoleColor.Green;
IReceiver? receiver=null;

string[] arguments = Environment.GetCommandLineArgs();
if (arguments.Length > 1) {
    string feature = arguments[1].ToLower();
    switch (feature) 
    {
        case "relay":
            receiver= await CreateRelayReceiverAsync(resourceClient, parametersSection);
            break;
        case "servicebus":
            receiver= await CreateServiceBusReceiverAsync(resourceClient,  parametersSection);
            break;
        case "webpubsub":
            receiver = await CreateWebSocketReceiverAsync(resourceClient, parametersSection);
            break;
        case "storagequeue":
            receiver = await CreateStorageQueueReceiver(resourceClient, parametersSection);
            break;
        default:
            Console.WriteLine("Unknow receiver : relay, servicebus, webpubsub, storagequeue ");
            break;
    }
}
else
{
    Console.WriteLine("Unknow receiver : relay, servicebus, webpubsub, storagequeue ");

}



if (receiver != null) 
{
    receiver.Wait = Wait;
    receiver.Response = ReturnMessage;
    await receiver.StartAsync();
}

static async Task<IReceiver?> CreateStorageQueueReceiver(ResourceClient resourceclient,
                                                         IConfigurationSection parametersection)
{
    string? account = parametersection["accountname:value"];
    if (account == null) { throw new NullReferenceException(nameof(account)); }
    string? queueName = parametersection["storagequeue:value"];
    if (queueName == null) { throw new NullReferenceException(nameof(queueName)); }



    string? connectionString = await resourceclient.GetStorageConnectionStringAsync(account);
    if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }

    return new StorageQueue(connectionString, queueName);
}
static async Task<IReceiver?> CreateWebSocketReceiverAsync(ResourceClient resourceclient, 
                                                           IConfigurationSection parametersection)
{
    string? webPubSub = parametersection["webpubsub:value"];
    if (webPubSub == null) { throw new NullReferenceException(nameof(webPubSub)); }
    string? hubname = parametersection["webpubsubhubname:value"];
    if (hubname == null) { throw new NullReferenceException(nameof(hubname)); }


    
    string? connectionString = await resourceclient.GetWebPubSubConnectionStringAsync(webPubSub);
    if (connectionString == null) { throw new NullReferenceException(nameof(connectionString)); }

    return new WebSocket(connectionString, hubname);
};

static async Task<IReceiver> CreateServiceBusReceiverAsync(ResourceClient resourceclient,                                                           
                                                           IConfigurationSection parameterssection)
{
    
    if (parameterssection== null) { throw new ArgumentNullException(nameof (parameterssection)); }

    string? serviceBus = parameterssection["servicebus:value"];
    if (serviceBus == null) { throw new NullReferenceException(nameof(serviceBus)); }

    string? serviceBusQueue = parameterssection["servicebusqueue:value"];
    if (serviceBusQueue == null) { throw new NullReferenceException(nameof(serviceBusQueue)); }

    //Get Key
    string? sasKeyName = parameterssection["saskeyname:value"];
    if (sasKeyName == null) { throw new NullReferenceException(nameof(sasKeyName)); }

    string connectionString = await resourceclient.GetServiceBusConnectionStringAsync(serviceBus, sasKeyName);
    
    return new ServiceBus(serviceBus, serviceBusQueue, connectionString);


}

static async Task<IReceiver> CreateRelayReceiverAsync(ResourceClient resourceclient,                                         
                                                      IConfigurationSection parameterssection)
                                        
{
    string? hybridConnection = parameterssection["hybridconnection:value"];
    if (hybridConnection == null) { throw new NullReferenceException(nameof(hybridConnection)); }

    string? relayNameSpace = parameterssection["relaynamespace:value"];
    if (relayNameSpace == null) { throw new NullReferenceException(nameof(relayNameSpace)); }
    
    string? sasKeyName = parameterssection["saskeyname:value"];
    if (sasKeyName == null) { throw new NullReferenceException(nameof(sasKeyName)); }
    
    string key = await resourceclient.GetRelayKeyAsync(relayNameSpace, hybridConnection, sasKeyName);
    
    return new Relay(relayNameSpace, hybridConnection, sasKeyName, key);
    
}