using Azure.Messaging.ServiceBus;

namespace Fx.Injector
{
    public class ServiceBus : IInjector
    {
        ServiceBusSender? _sender;
        public string InjectorType { get; }
        

        public ServiceBus(string connectionstring,string queue) { 
            
            
            ServiceBusClient serviceBus=new ServiceBusClient(connectionstring);
            _sender = serviceBus.CreateSender(queue);
            InjectorType = "ServiceBus";            
        }
        public async Task SendAsync(object message)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
            if (_sender == null) { throw new NullReferenceException(nameof(_sender)); }


            ServiceBusMessage serviceBusMessage = new ServiceBusMessage
            {
                Body = BinaryData.FromString(message.ToString())
            };
            await _sender.SendMessageAsync(serviceBusMessage);
                        
        }
    }
}