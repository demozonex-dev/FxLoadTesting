

using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{
    public class WebSocket : Base,IReceiver
    {
        const string MESSAGE = "Connected to Azure WebSocket PubSub";

        public object Parameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IConfiguration? Configuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task Start()
        {
            Wait(MESSAGE);
        }
        public Task Stop()
        {
            return Task.CompletedTask;
        }
    }
    
}