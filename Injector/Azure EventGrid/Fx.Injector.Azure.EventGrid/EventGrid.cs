using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Azure.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;

namespace Fx.Injector
{
    public class EventGrid : IInjector
    {
        private readonly IConfiguration _configuration;
        
        private readonly EventGridPublisherClient _client;
        public EventGrid(IConfiguration configuration) {
            _configuration = configuration;
            string topicKey = _configuration["Key"];
            string topicEndpoint = _configuration["TopicEndpoint"];
            
            _client = new EventGridPublisherClient(
                                            new Uri(topicEndpoint),
                                            new Azure.AzureKeyCredential(topicKey));
        }
        public async Task Send(object data)
        {
            
            await _client.SendEventAsync(new EventGridEvent("New data",
                                        "NEW_DATA",
                                        "V1.0",
                                        data));


        }
    }
}