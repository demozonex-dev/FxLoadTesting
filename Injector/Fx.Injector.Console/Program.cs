﻿// See https://aka.ms/new-console-template for more information
using Azure.Core;
using Azure.Identity;
using Fx.ArmManager;
using Fx.Injector;
using Microsoft.Extensions.Configuration;


await SendMessageToEventGrid(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode));


static async Task SendMessageToEventGrid(TokenCredential credential)
{
    Console.WriteLine("Send Message to Event Grid");
    Console.WriteLine("Enter any key so continue");
    Console.ReadLine();

    //Get the parameters for the EventGrid from files
    //var config = Fx.Helpers.Configuration.Create();
    //string topicKey = config["Key"];
    //string topicEndpoint = config["TopicEndpoint"];
    //IInjector injector = new Fx.Injector.EventGrid(new Uri(topicEndpoint),topicKey));

    
    //Get the resources's names
    var config = Fx.Helpers.Configuration.Create();
    var parameterSection= config.GetSection("parameters");
    string? resourceGroupName = parameterSection["resourcegroup:value"];
    if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }

    
    string? topicName = parameterSection["eventgridtopic:value"];
    if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }


    // Get EndPoint and Key from Azure, so no need to stock the secrets
    ResourceClient resourceClient = new ResourceClient();
    await resourceClient.EasyInitAsync(resourceGroupName,credential);

    var eventGridConnecyionInfos = await resourceClient.GetEventGridConnectionInfosAsync(topicName);

    IInjector injector = new Fx.Injector.EventGrid(eventGridConnecyionInfos.endpoint, eventGridConnecyionInfos.key);

    Data data = new Data
    {
        Id = Guid.NewGuid().ToString(),
        Description= "Démonstration d'un message envoyé via EventGrid",        
        Date = DateTime.Now
    };
    for(int i=1;i<=5;i++)
    {
        await injector.SendAsync(data);
        Console.WriteLine($"Message {i} Sent");
    }
    

    
}

class Data
{    
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
};
