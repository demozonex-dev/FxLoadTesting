using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.ComponentModel.Design;
using System.Net.WebSockets;
using System.Text;

namespace Fx.Receiver
{
    public class StorageQueue : ReceiverBase, IReceiver
    {
        const string MESSAGE = "Connected to Azure Storage Queue, waiting for message";
        public string ReceiverType { get; }

        QueueClient _queueClient;
        public StorageQueue(string connectionstring, string queuename) 
        { 
            if (connectionstring == null) { throw new ArgumentNullException(nameof(connectionstring)); }
            if (queuename == null) { throw new ArgumentNullException(nameof(queuename));}
            ReceiverType = "StorageQueue";
            _queueClient = new QueueClient(connectionstring,queuename);
        }
        public async Task StartAsync()
        {
            if (_queueClient == null) { throw new NullReferenceException(nameof(_queueClient)); }
            if (Wait == null) throw new NullReferenceException(nameof(Wait));
            if (Response == null) throw new NullReferenceException(nameof(Response));

            var cts = new CancellationTokenSource();

            await Task.Factory.StartNew(
                async () =>
                {
                    //Pas bo
                    while (true)
                    {
                        string rcvMsg = await RetrieveNextMessageAsync(_queueClient);
                        if (rcvMsg == null)
                        {
                            
                        }
                        else
                        {
                            Response(rcvMsg);
                        }

                            
                        
                    }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Wait(MESSAGE);

        }
        static async Task<string> RetrieveNextMessageAsync(QueueClient theQueue)
        {

            QueueProperties properties = await theQueue.GetPropertiesAsync();

            if (properties.ApproximateMessagesCount > 0)
            {
                //This is a no blocking call so find an elegante way to avoid pooling the queue
                QueueMessage[] retrievedMessage = await theQueue.ReceiveMessagesAsync(1, new TimeSpan(0, 0, 30));
                string theMessage = retrievedMessage[0].Body.ToString();
                await theQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                return theMessage;
            }

            return null;
        }
        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}