// See https://aka.ms/new-console-template for more information

using Azure.ResourceManager.Storage;
using Fx.ArmManager;
using Fx.CreateInfra;

//ResourceClient resourceClient = new ResourceClient();
//resourceClient.Login(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.BrowserInteractive));
//await resourceClient.SetDefaultSubscriptionAsync();

ResourceClient resourceClient = await Helper.LoginToAzureAsync();

var config = Fx.Helpers.Configuration.Create();

var parametersSection = config.GetSection("parameters");
string? location = await Helper.CreateResourceGroupAsync(resourceClient, parametersSection);
if (location == null) { throw new NullReferenceException(nameof(location)); }
Console.Clear();
Console.ForegroundColor = ConsoleColor.Green;

Console.WriteLine("Choose the PAAS service you want to create");
Console.WriteLine("1 - Create Event Grid with Relay and Service Bus");
Console.WriteLine("2 - Create Web Pub Sub");
Console.WriteLine("3 - Create Storage Queue");
Console.WriteLine("4 - Create Event Hub");
Console.WriteLine("5 - Create Only Service Bus");
//Console.WriteLine("9 - Create App Service Plan");
Console.WriteLine("y - Delete all resources");
Console.WriteLine("z - Create all services");
ConsoleKeyInfo key = Console.ReadKey();
System.Console.Clear();
switch (key.KeyChar)
{
    
    case '1':
        await Helper.CreateRelayAsync(resourceClient, parametersSection, location);
        await Helper.CreateServiceBusAsync(resourceClient, parametersSection, location);
        await Helper.CreateEventGridAsync(resourceClient, parametersSection, location);
        break;
    case '2':
        await Helper.CreateWebPubSubAsync(resourceClient, parametersSection, location);
        break;
    case '3':
        await Helper.CreateStorageAccountAsync(resourceClient, parametersSection, location);
        break;
    case '4':
        Console.WriteLine("not implemented yet");
        break;
    case '5':
        await Helper.CreateServiceBusAsync(resourceClient, parametersSection, location);
        break;
    case '9':
        await Helper.CreateAppServicePlan(resourceClient, parametersSection, location);
        break;
    case 'z':
    case 'Z':
        await Helper.CreateRelayAsync(resourceClient, parametersSection, location);
        await Helper.CreateServiceBusAsync(resourceClient, parametersSection, location);
        await Helper.CreateEventGridAsync(resourceClient, parametersSection, location);
        await Helper.CreateWebPubSubAsync(resourceClient, parametersSection, location);
        await Helper.CreateStorageAccountAsync(resourceClient, parametersSection, location);
        await Helper.CreateStorageAccountAsync(resourceClient, parametersSection, location);
        await Helper.CreateAppServicePlan(resourceClient, parametersSection, location);
        break;
    default:
        
        break;
}

Console.WriteLine("Success !!!!");

