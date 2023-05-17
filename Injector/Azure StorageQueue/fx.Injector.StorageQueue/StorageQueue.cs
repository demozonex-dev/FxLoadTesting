using Azure.Storage.Queues;
using Fx.Injector;

namespace fx.Injector
{
    public class StorageQueue : IInjector
    {
        public string InjectorType { get; }
        QueueClient _queueClient;
        public StorageQueue(string connectionstring, string queuename)
        {

            if (connectionstring == null) { throw new ArgumentNullException(nameof(connectionstring)); }
            if (queuename == null) { throw new ArgumentNullException(nameof(queuename)); }
            
            _queueClient = new QueueClient(connectionstring, queuename);
            InjectorType = "StorageQueue";

        }
        public async Task SendAsync(object message)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
            if (_queueClient == null) { throw new NullReferenceException(nameof(_queueClient)); }
            await _queueClient.SendMessageAsync(message.ToString());
        }
    }
}