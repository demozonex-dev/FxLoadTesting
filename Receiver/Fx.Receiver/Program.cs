// See https://aka.ms/new-console-template for more information

using Fx.Receiver;

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


string hybridConnection = "hcdeployment";
string sasKeyName = "RootManageSharedAccessKey";
string key = "ob1a86m5cn7v9WQBEx8TEDNxx3Q9+C39L+ARmFOKj1Y=";
string relayNameSpace = "gridrelay100.servicebus.windows.net";

IReceiver receiver = new Relay(relayNameSpace,hybridConnection,sasKeyName,key);



receiver.Wait = Wait;
receiver.ReturnMessage = ReturnMessage;
await receiver.Start();


