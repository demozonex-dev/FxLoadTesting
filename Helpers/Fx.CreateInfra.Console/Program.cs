// See https://aka.ms/new-console-template for more information

using Azure.ResourceManager.AppService.Models;
using Fx.ArmManager;
using Microsoft.Extensions.Configuration;

ResourceClient resourceClient = new ResourceClient();
resourceClient.Login(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.BrowserInteractive));
await resourceClient.SetDefaultSubscriptionAsync();

var config = Fx.Helpers.Configuration.Create();
var parametersSection = config.GetSection("parameters");
string? resourceGroupName = parametersSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }
//string? location = parametersSection["location:value"];
string? location = await CreateResourceGroupAsync(resourceClient, parametersSection);
if (location == null) { throw new NullReferenceException(nameof(location)); }

await resourceClient.SetDefaultResourceGroupAsync(resourceGroupName);

//Execute in order because EventGrid depend on Relay and Service bus for the subscription
await CreateRelayAsync(resourceClient, parametersSection, location);
await CreateServiceBusAsync(resourceClient, parametersSection, location);
await CreateEventGridAsync(resourceClient, parametersSection, location);
//await CreateAppServicePlan(resourceClient, parametersSection, location);
Console.WriteLine("Success !!!!");

static async Task CreateAppServicePlan(ResourceClient resourceClient, IConfiguration  parametersection, string location)
{
    
    string? appServicePlan = parametersection["appserviceplan:value"];
    if (appServicePlan == null) { throw new NullReferenceException(nameof(appServicePlan)); }
    
    string? kind= parametersection["appserviceplan:kind"];
    if (kind == null)
    {
        kind = "linux";
    }

    var  sectionSku = parametersection.GetSection("appserviceplan:sku");
    if (sectionSku == null) { throw new NullReferenceException(nameof(sectionSku)); }
    

    AppServiceSkuDescription? appSkuDescription = new AppServiceSkuDescription
    {
        Name = sectionSku["name"],
        Tier= sectionSku["tier"],
        Size= sectionSku["size"],
        Family = sectionSku["family"],
        Capacity = int.Parse(sectionSku["capacity"])        
    };
    Console.WriteLine("Creating App Service Plan");
    
    await resourceClient.CreateOrUpdateAppService(appServicePlan,appSkuDescription,kind, location);
}

static async Task<string?> CreateResourceGroupAsync(ResourceClient resourceClient,
                                               IConfigurationSection parametersection)
{
    string? location = parametersection["location:value"];
    if (location == null) { throw new NullReferenceException(nameof(location)); }

    string? resourceGroupName = parametersection["resourcegroup:value"];
    if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

    Console.WriteLine("Creating Resource Group");
    await resourceClient
                .CreateOrUpdateResourceGroupAsync(resourceGroupName,
                                                  location);
    return location;
}

static async Task CreateRelayAsync(ResourceClient resourceClient, 
                                   IConfigurationSection parametersSection, 
                                   string? location)
{
    string? hybridConnection = parametersSection["hybridconnection:value"];
    if (hybridConnection == null) { throw new NullReferenceException(nameof(hybridConnection)); }
    string? relayNamespace = parametersSection["relaynamespace:value"];
    if (relayNamespace == null) { throw new NullReferenceException(nameof(relayNamespace)); }
    if (location == null) { throw new NullReferenceException(nameof(location)); }

    Console.WriteLine("Creating Relay");
    await resourceClient
                .CreateOrUpdateRelayNamespaceAsync(relayNamespace,
                                                   hybridConnection,
                                                   location);
}

static async Task CreateEventGridAsync(ResourceClient resourceClient, 
                                       IConfigurationSection parametersSection, 
                                       string? location)
{
    string? topicName = parametersSection["eventgridtopic:value"];
    if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }
    string? subscriptionName = parametersSection["eventgridsubscription:value"];
    if (subscriptionName == null) { throw new NullReferenceException(nameof(subscriptionName)); }
    if (location == null) { throw new NullReferenceException(nameof(location)); }

    Console.WriteLine("Creating event grid");
    await resourceClient
                       .CreateOrUpdateEventGridTopicAsync(topicName,
                                                          subscriptionName,
                                                          location);
}

static async Task CreateServiceBusAsync(ResourceClient resourceClient, 
                                        IConfigurationSection parametersSection, 
                                        string? location)
{
    string? serviceBus = parametersSection["servicebus:value"];
    if (serviceBus == null) { throw new NullReferenceException(nameof(serviceBus)); }

    string? serviceBusQueue = parametersSection["servicebusqueue:value"];
    if (serviceBusQueue == null) { throw new NullReferenceException(nameof(serviceBusQueue)); }

    string? serviceBusTopic = parametersSection["servicebustopic:value"];
    if (serviceBusTopic == null) { throw new NullReferenceException(nameof(serviceBusTopic)); }
    if (location == null) { throw new NullReferenceException(nameof(location)); }
    Console.WriteLine("Creating ServiceBus");
    await resourceClient.CreateOrUpdateServiceBusAsync(serviceBus, serviceBusQueue, serviceBusTopic, location);
}