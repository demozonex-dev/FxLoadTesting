// See https://aka.ms/new-console-template for more information

using Fx.Receiver;
using Microsoft.Extensions.Configuration;
using System.Reflection;

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
    Console.WriteLine(message);
};



IReceiver receiver = new Relay(CreateConfiguration());



receiver.Wait = Wait;
receiver.ReturnMessage = ReturnMessage;
await receiver.Start();

IConfigurationRoot CreateConfiguration()
{
    ConfigurationBuilder configuration = new ConfigurationBuilder();
    configuration.AddJsonFile("appsettings.json", true, true);
    
    //Read the user secrets from the secrets.json files
    //The id change for each pc
    configuration.AddUserSecrets("fa016515-f620-4f83-a9c5-67501e23557e");
    return configuration.Build();
}


IReceiver CreateReceiver42(string path, string type)
{
    //Read from AppSettings.json
    //var assembly = Assembly.LoadFile(path);
    var assembly = Assembly.LoadFrom(path);

    return (IReceiver)assembly.CreateInstance(type);

}