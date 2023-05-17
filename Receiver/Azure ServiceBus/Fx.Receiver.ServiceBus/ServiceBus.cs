using Azure;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using System.Net;
using System.Runtime.CompilerServices;

namespace Fx.Receiver
{
    
    public class ServiceBus : ReceiverBase, IReceiver
    {
        const string MESSAGE = "Connected to Azure Service Bus, waiting for message";
        ServiceBusProcessor? _processor;
        public string ReceiverType { get; }
        public ServiceBus(string servicebus, string queue, string connectionstring)
        {
            if (servicebus == null) { throw new ArgumentNullException(nameof(servicebus)); }
            if (queue == null) { throw new ArgumentNullException(nameof(queue)); }
            if (connectionstring == null) { throw new ArgumentNullException(nameof(connectionstring)); }

            ReceiverType = "ServiceBus";
            
            ServiceBusClient serviceBusClient = new ServiceBusClient(connectionstring);
            if (serviceBusClient == null) { throw new NullReferenceException(nameof(serviceBusClient)); }

            CreateProcessor(serviceBusClient, queue);
            
        }        
        public ServiceBus(string servicebus, string queue, TokenCredential credential) 
        { 
            if (servicebus == null) { throw new ArgumentNullException(nameof(servicebus)); }
            if (queue == null) { throw new ArgumentNullException(nameof(queue));    }
            if (credential==null) { throw new ArgumentNullException(nameof(credential)); }
            
            //TODO: Give RBAC Access if you use tokenCredentiel
            ServiceBusClient serviceBusClient =new ServiceBusClient(servicebus,credential);
            CreateProcessor(serviceBusClient, queue);

        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            string MessageReceive = arg.Exception.Message;
            if (Response != null)
            {
                Response(MessageReceive);
            }
            return Task.CompletedTask;
        }

        private async Task _processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));
            string MessageReceive = arg.Message.Body.ToString();
            if (Response!=null)
            {
                Response(MessageReceive);
            }
            await arg.CompleteMessageAsync(arg.Message);
        }
        private void CreateProcessor(ServiceBusClient servicebusclient, string queue)
        {
            if (servicebusclient == null) { throw new ArgumentNullException(nameof(servicebusclient)); }

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentCalls = 1
            };
            _processor = servicebusclient.CreateProcessor(queue, options);
            if (_processor == null) { throw new NullReferenceException(nameof(_processor)); }

            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync;

        }
        public async Task StartAsync()
        {
            if (Wait == null) throw new NullReferenceException(nameof(Wait));
            if (Response == null) throw new NullReferenceException(nameof(Response));
            if (_processor == null) { throw new NullReferenceException(nameof(_processor)); }   

            await _processor.StartProcessingAsync();
            Wait(MESSAGE);
            await _processor.StopProcessingAsync();
        }

        public async Task StopAsync()
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
            }
        }
    }
}