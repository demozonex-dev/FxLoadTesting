using Microsoft.Extensions.Configuration;

namespace Fx.Helpers
{
    public class Configuration
    {
        public static IConfigurationRoot Create()
        {
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.AddJsonFile("appsettings.json", true, true);

            //Read the user secrets from the secrets.json files
            //The id change for each project
            configuration.AddUserSecrets("fa016515-f620-4f83-a9c5-67501e23557e");
            configuration.AddUserSecrets("fd61f8e8-dbdc-47ed-a235-7194143efdf4");
            return configuration.Build();
        }
    }
}
