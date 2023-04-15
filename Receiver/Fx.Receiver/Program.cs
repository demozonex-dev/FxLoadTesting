// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Fx.ArmManager;
using Fx.Receiver;


TokenCredential credential = await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode);
await RelayReceiver(credential);

static async Task RelayReceiver(TokenCredential credential)
{
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
        Console.WriteLine("new message");
        Console.WriteLine(message);
    };


    var config = Fx.Helpers.Configuration.Create();
    var parametersSection = config.GetSection("parameters");
    string? resourceGroupName = parametersSection["resourcegroup:value"];
    if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

    
    string? hybridConnection = parametersSection["hybridconnection:value"];
    if (hybridConnection == null) { throw new NullReferenceException(nameof(hybridConnection)); }

    string? relayNameSpace = parametersSection["relaynamespace:value"];
    if (relayNameSpace == null) { throw new NullReferenceException(nameof(relayNameSpace)); }

    string? sasKeyName = parametersSection["saskeyname:value"];
    if (sasKeyName == null) { throw new NullReferenceException(nameof(sasKeyName)); }

    //TODO : Test if Linux or Windows to authenticate with the right Credential
    ResourceClient resourceClient = new ResourceClient();
    await resourceClient.EasyInitAsync(resourceGroupName, credential);

    string key = await resourceClient.GetRelayKeyAsync(relayNameSpace, hybridConnection, sasKeyName);

    IReceiver receiver = new Relay(relayNameSpace, hybridConnection, sasKeyName, key);

    receiver.Wait = Wait;
    receiver.Response = ReturnMessage;
    await receiver.StartAsync();

}



