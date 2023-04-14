// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Azure.Identity;
using Fx.ArmManager;
using Fx.Receiver;
using Microsoft.Extensions.Configuration;


await RelayReceiver(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode));

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
    string? resourceGroupName = config["resourcegroup"];
    if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

    IConfigurationSection? section = config.GetSection("relay");
    if (section == null) { throw new NullReferenceException(nameof(section)); }

    string? hybridConnection = section["hybridconnection"];
    if (hybridConnection == null) { throw new NullReferenceException(nameof(hybridConnection)); }

    string? relayNameSpace = section["relaynamespace"];
    if (relayNameSpace == null) { throw new NullReferenceException(nameof(relayNameSpace)); }

    string? sasKeyName = section["saskeyname"];
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



