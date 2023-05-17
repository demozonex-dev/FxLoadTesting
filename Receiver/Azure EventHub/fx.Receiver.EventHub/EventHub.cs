using Fx.Receiver;

namespace fx.Receiver
{
    public class EventHub : ReceiverBase, IReceiver
    {
        public string ReceiverType => throw new NotImplementedException();

        public Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}