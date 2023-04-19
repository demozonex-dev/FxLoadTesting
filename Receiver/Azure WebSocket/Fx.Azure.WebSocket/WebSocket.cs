

using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{
    public class WebSocket : ReceiverBase,IReceiver
    {
        const string MESSAGE = "Connected to Azure WebSocket PubSub";

        public object Parameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IConfiguration? Configuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ReceiverType => throw new NotImplementedException();

        public async Task StartAsync()
        {
            Wait(MESSAGE);
        }
        public Task Stop()
        {
            return Task.CompletedTask;
        }
    }
    
}