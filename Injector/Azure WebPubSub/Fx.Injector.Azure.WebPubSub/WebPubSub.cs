using Azure.Messaging.WebPubSub;

namespace Fx.Injector
{
    public class WebPubSub : IInjector
    {
        public string InjectorType { get; }
        WebPubSubServiceClient _webPubSubServiceClient;
        public WebPubSub(string connectionstring, string hubname)
        {
            if (connectionstring == null) { throw new ArgumentNullException(nameof(connectionstring)); }
            if (hubname == null) { throw new ArgumentNullException(nameof(hubname)); }

            _webPubSubServiceClient =
                new WebPubSubServiceClient(connectionstring,
                                           hubname,
                                           new WebPubSubServiceClientOptions
                                           {

                                           });
            InjectorType = "WebPubSub";
        }
        public async Task SendAsync(object message)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
            if (_webPubSubServiceClient == null) { throw new NullReferenceException(nameof(_webPubSubServiceClient)); }
            await _webPubSubServiceClient.SendToAllAsync(message.ToString());

        }
    }
}