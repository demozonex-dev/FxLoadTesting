// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Azure.ResourceManager.ServiceBus.Models;
using Fx.ArmManager;
using Fx.Models;
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


var config = Fx.Helpers.Configuration.Create();
var parametersSection = config.GetSection("parameters");
string? resourceGroupName = parametersSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

TokenCredential credential = await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode);
ResourceClient resourceClient = new ResourceClient();
await resourceClient.EasyInitAsync(resourceGroupName, credential);
IReceiver? receiver=null;

string[] arguments = Environment.GetCommandLineArgs();
if (arguments.Length > 1) { 
    switch (arguments[1]) 
    {
        case "relay":
            receiver= await CreateRelayReceiverAsync(resourceClient, parametersSection);
            break;
        case "servicebus":
            receiver= await CreateServiceBusReceiverAsync(resourceClient, credential, parametersSection);
            break;
        default:
            Console.WriteLine("Unknow receiver : relay, servicebus");
            break;
    }
}
else
{
    Console.WriteLine("missing receiver : relay, servicebus");
    
};

if (receiver != null) 
{
    receiver.Wait = Wait;
    receiver.Response = ReturnMessage;
    await receiver.StartAsync();
}



static async Task<IReceiver> CreateServiceBusReceiverAsync(ResourceClient resourceclient,TokenCredential? credential, IConfigurationSection parameterssection)
{
    if (credential == null) { throw new ArgumentNullException(nameof(credential)); }
    if (parameterssection== null) { throw new ArgumentNullException(nameof (parameterssection)); }

    string? serviceBus = parameterssection["servicebus:value"];
    if (serviceBus == null) { throw new NullReferenceException(nameof(serviceBus)); }

    string? serviceBusQueue = parameterssection["servicebusqueue:value"];
    if (serviceBusQueue == null) { throw new NullReferenceException(nameof(serviceBusQueue)); }

    //Get Key
    string? sasKeyName = parameterssection["saskeyname:value"];
    if (sasKeyName == null) { throw new NullReferenceException(nameof(sasKeyName)); }

    string connectionString = await resourceclient.GetServiceBusConnectionStringAsync(serviceBus, sasKeyName);
    //return new ServiceBus(serviceBus,serviceBusQueue, credential);
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