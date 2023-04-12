using Azure.Messaging.EventGrid;


namespace Fx.Injector
{
    public class EventGrid : IInjector
    {
       
        
        private readonly EventGridPublisherClient _client;
       
        public EventGrid(Uri endpoint, string key)
        {
            if (endpoint == null) { throw new ArgumentNullException(nameof(endpoint)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            _client = new EventGridPublisherClient(
                                            endpoint,
                                            new Azure.AzureKeyCredential(key));
        }

        
        public async Task SendAsync(object data)
        {
            if (_client == null) { throw new NullReferenceException(nameof(_client)); }
        
            var response= await _client.SendEventAsync(new EventGridEvent("New data",
                                        "NEW_DATA",
                                        "V1.0",
                                        data));
        }
    }
}