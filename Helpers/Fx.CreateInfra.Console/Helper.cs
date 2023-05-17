using Azure.ResourceManager.AppService.Models;
using Fx.ArmManager;
using Microsoft.Extensions.Configuration;
using System;
namespace Fx.CreateInfra
{
    internal static class Helper
    {
        internal static async Task CreateAppServicePlan(ResourceClient resourceClient,
                                       IConfiguration parametersection, string location)
        {

            string? appServicePlan = parametersection["appserviceplan:value"];
            if (appServicePlan == null) { throw new NullReferenceException(nameof(appServicePlan)); }

            string? kind = parametersection["appserviceplan:kind"];
            if (kind == null)
            {
                kind = "linux";
            }

            var sectionSku = parametersection.GetSection("appserviceplan:sku");
            if (sectionSku == null) { throw new NullReferenceException(nameof(sectionSku)); }

            

            AppServiceSkuDescription? appSkuDescription = new AppServiceSkuDescription
            {
                Name = sectionSku["name"],
                Tier = sectionSku["tier"],
                Size = sectionSku["size"],
                Family = sectionSku["family"],
                Capacity = int.Parse(sectionSku["capacity"])
            };

            Printf($"{appServicePlan} App Service Plan");
            await resourceClient.CreateOrUpdateAppService(appServicePlan, appSkuDescription, kind, location);
        }

        internal static async Task<string?> CreateResourceGroupAsync(ResourceClient resourceClient,
                                                       IConfigurationSection parametersection)
        {
            string? location = parametersection["location:value"];
            if (location == null) { throw new NullReferenceException(nameof(location)); }

            string? resourceGroupName = parametersection["resourcegroup:value"];
            if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

            Printf($"{resourceGroupName} Resource Group");
            await resourceClient
                        .CreateOrUpdateResourceGroupAsync(resourceGroupName,
                                                          location);
            await resourceClient.SetDefaultResourceGroupAsync(resourceGroupName);
            return location;
        }

        internal static async Task CreateRelayAsync(ResourceClient resourceClient,
                                           IConfigurationSection parametersSection,
                                           string? location)
        {
            
            string? hybridConnection = parametersSection["hybridconnection:value"];
            if (hybridConnection == null) { throw new NullReferenceException(nameof(hybridConnection)); }
            string? relayNamespace = parametersSection["relaynamespace:value"];
            if (relayNamespace == null) { throw new NullReferenceException(nameof(relayNamespace)); }
            if (location == null) { throw new NullReferenceException(nameof(location)); }

            
            Printf($"{hybridConnection} Relay");
            await resourceClient
                        .CreateOrUpdateRelayNamespaceAsync(relayNamespace,
                                                           hybridConnection,
                                                           location);
        }

        internal static async Task CreateEventGridAsync(ResourceClient resourceClient,
                                               IConfigurationSection parametersSection,
                                               string? location)
        {
            string? topicName = parametersSection["eventgridtopic:value"];
            if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }
            string? subscriptionName = parametersSection["eventgridsubscription:value"];
            if (subscriptionName == null) { throw new NullReferenceException(nameof(subscriptionName)); }
            if (location == null) { throw new NullReferenceException(nameof(location)); }

            Printf($"{topicName} Event Grid Topic");
            await resourceClient
                               .CreateOrUpdateEventGridTopicAsync(topicName,
                                                                  subscriptionName,
                                                                  location);
        }
        
        internal static async Task CreateServiceBusAsync(ResourceClient resourceClient,
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
            
            Printf($"{serviceBus} Service Bus");

            await resourceClient.CreateOrUpdateServiceBusAsync(serviceBus, serviceBusQueue, serviceBusTopic, location);
        }

        internal static async Task CreateWebPubSubAsync(ResourceClient resourceclient,
                                               IConfigurationSection parametersection,
                                               string? location)
        {
            string? webPubSub = parametersection["webpubsub:value"];
            if (webPubSub == null) { throw new NullReferenceException(nameof(webPubSub)); }
            string? hubname = parametersection["webpubsubhubname:value"];
            if (hubname == null) { throw new NullReferenceException(nameof(hubname)); }
            if (location == null) { throw new NullReferenceException(nameof(location)); }
            Printf($"{webPubSub} Web PubSub");
            await resourceclient.CreateOrUpdateWebPubSubAsync(webPubSub, hubname, location);
        }

        internal static async Task CreateStorageAccountAsync(ResourceClient resourceclient,
                                               IConfigurationSection parametersection,
                                               string? location)
        {
            string? account = parametersection["accountname:value"];
            if (account == null) { throw new NullReferenceException(nameof(account)); }
            string? queueName = parametersection["storagequeue:value"];
            if (queueName == null) { throw new NullReferenceException(nameof(queueName)); }
            if (location == null) { throw new NullReferenceException(nameof(location)); }
            Printf($"{account} Storage Account");
            await resourceclient.CreateOrUpdateStorageAccountAsync(account, queueName, location);
        }
      
        internal static async Task<ResourceClient> LoginToAzureAsync()
        {
            Printf("Login to Azure....");
            ResourceClient resourceClient = new ResourceClient();
            resourceClient.Login(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.BrowserInteractive));
            await resourceClient.SetDefaultSubscriptionAsync();
            return resourceClient;

        }
        private static void Printf(string message)
        {
            
            System.Console.WriteLine($"Creating/Updating {message} ...");
        }
    }
}
