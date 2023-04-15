// See https://aka.ms/new-console-template for more information

using Azure.Identity;
using Fx.ArmManager;



ResourceClient resourceClient = new ResourceClient();
resourceClient.Login(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode));
await resourceClient.SetDefaultSubscriptionAsync();

var config = Fx.Helpers.Configuration.Create();
var parametersSection = config.GetSection("parameters");
string? resourceGroupName = parametersSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }


string? location = parametersSection["location:value"];
if (location==null) { throw new NullReferenceException(nameof(location)); }
Console.WriteLine("Creating Resource Group");
await resourceClient
            .CreateOrUpdateResourceGroupAsync(resourceGroupName, 
                                              location);

string? topicName = parametersSection["eventgridtopic:value"];
if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }

string? subscriptionName = parametersSection["eventgridsubscription:value"];
if (subscriptionName == null) { throw  new NullReferenceException(nameof(subscriptionName)); }

string? hybridConnection = parametersSection["hybridconnection:value"];
if (hybridConnection==null) { throw new NullReferenceException(nameof(hybridConnection)); }
string? relayNamespace = parametersSection["relaynamespace:value"];
if (relayNamespace==null) { throw new NullReferenceException(nameof(relayNamespace)); }

Console.WriteLine("Creating Relay");
await resourceClient
            .CreateOrUpdateRelayNamespaceAsync(relayNamespace,
                                               hybridConnection,
                                               location);
Console.WriteLine("Creating event grid");
await resourceClient
                   .CreateOrUpdateEventGridTopicAsync(topicName,
                                                      subscriptionName,
                                                      location);

Console.WriteLine("Success !!!!");