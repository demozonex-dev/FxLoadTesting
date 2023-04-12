// See https://aka.ms/new-console-template for more information
using Azure.Identity;
using Azure.ResourceManager.Resources;
using Fx.ArmManager;
using Fx.Helpers;
using Fx.Injector;

Console.WriteLine("Send Message to Event Grid");
Console.WriteLine("Enter any key so continue");
Console.ReadLine();

//Get the config from files
//var config = Fx.Helpers.Configuration.Create();
//string topicKey = config["Key"];
//string topicEndpoint = config["TopicEndpoint"];
//IInjector injector = new Fx.Injector.EventGrid(new Uri(topicEndpoint),topicKey));

//Get the resources's names
var config = Fx.Helpers.Configuration.Create();
string? resourceGroupName = config["resourcegroup"];
var section=config.GetSection("eventgrid");
var topicName = section["topicname"];

// Get EndPoint and Key dynamicly
ResourceClient resourceClient = new ResourceClient();
resourceClient.Login(new VisualStudioCredential());
resourceClient.SetDefaultSubscription();
resourceClient.SetDefaultResourceGroup(resourceGroupName);
var eventGridConnecyionInfos = await resourceClient.GetEventGridConnectionInfos(topicName);

IInjector injector = new Fx.Injector.EventGrid(eventGridConnecyionInfos.endpoint, eventGridConnecyionInfos.key);

Data data =  new Data
{
    Nom="Vernié",
    Prenom="Eric",
    Age=58,
    Date=DateTime.Now
};

await injector.Send(data);

Console.WriteLine("Message Sent");

class Data
{
    public int Age { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public DateTime Date { get; set; }
};

