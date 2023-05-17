// See https://aka.ms/new-console-template for more information
using Azure.Core;
using Fx.ArmManager;
using Fx.Injector;
using System.Text.Json;

TokenCredential tokenCredential = await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode);
var config = Fx.Helpers.Configuration.Create();
var parameterSection = config.GetSection("parameters");
string? resourceGroupName = parameterSection["resourcegroup:value"];
if (resourceGroupName == null) { throw new NullReferenceException(nameof(resourceGroupName)); }


ResourceClient resourceClient = new ResourceClient();
await resourceClient.EasyInitAsync(resourceGroupName, tokenCredential);
string[] arguments = Environment.GetCommandLineArgs();
int maxMessage = 2;
Console.ForegroundColor = ConsoleColor.Yellow;

IInjector? injector = null;
if (arguments.Length > 1)
{
    switch (arguments[1].ToLower())
    {
        case "eventgrid":
            injector = await Helper.CreateEventGridInjector(resourceClient, parameterSection);
            break;
        case "servicebus":            
            injector=await Helper.CreateServiceBusInjector(resourceClient, parameterSection);
            break;
        case "webpubsub":
            injector=await Helper.CreateWebPubSubInjector(resourceClient, parameterSection);
            break;
        case "storagequeue":
            injector=await Helper.CreateStorageQueueInjector(resourceClient, parameterSection);
            break;
        default:
            Console.WriteLine("Unknow injector : eventgrid, servicebus, webpubsub, storagequeue");
            break;
    }
}
else
{
    Console.WriteLine("Unknow injector : eventgrid, servicebus, webpubsub, storagequeue");
};
System.Console.WriteLine("Enter any key to send the messages");
Console.WriteLine("How many messages do you want to send ? (default 2) ");
string numMessage= Console.ReadLine(); 
int.TryParse(numMessage, out maxMessage);

if (injector != null)
{


    string host=Fx.Helpers.NetworkInfo.GetHostName();
    bool sendAnotherMessage = false;
    do
    {

        
        for (int i = 1; i <= maxMessage; i++)
        {
            Messages.DemoMessage data = new Messages.DemoMessage
            {
                Id = Guid.NewGuid().ToString(),
                Description = "message send",
                InjectorDate = DateTime.Now,
                Injector = injector.InjectorType,
                Host = host,
            };
            string jsonData = JsonSerializer.Serialize(data);
            await injector.SendAsync(jsonData);
            Console.WriteLine($"Message {i} Sent");
        }
        Console.WriteLine("Do you want to send more messages ? (y/n)");
        ConsoleKeyInfo key = Console.ReadKey();
        sendAnotherMessage = (key.KeyChar == 'y' || key.KeyChar == 'Y');
    }
    while (sendAnotherMessage);
}


