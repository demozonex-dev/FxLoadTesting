

using Microsoft.Extensions.Configuration;
using Azure.Messaging.WebPubSub;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Text;

namespace Fx.Receiver
{
    public class WebSocket : ReceiverBase,IReceiver
    {
        const string MESSAGE = "Connected to Azure WebSocket PubSub";

        public object Parameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IConfiguration? Configuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ReceiverType { get; }
        Uri webSocketUri { get; }
        WebPubSubServiceClient _webPubSubServiceClient;
        public WebSocket(string connectionstring, string hubname)
        {
            if (connectionstring == null) { throw new ArgumentNullException(nameof(connectionstring)); }
            if (hubname == null) { throw new ArgumentNullException(nameof(hubname)); }

             _webPubSubServiceClient =
                new WebPubSubServiceClient(connectionstring,
                                           hubname,
                                           new WebPubSubServiceClientOptions
                                           {
                                               
                                           });
            ReceiverType = "WebSocket";
            webSocketUri = _webPubSubServiceClient.GetClientAccessUri();
        }
        public async Task StartAsync()
        {
            if (Wait == null) throw new NullReferenceException(nameof(Wait));
            if (Response == null) throw new NullReferenceException(nameof(Response));
            if (_webPubSubServiceClient==null) throw new NullReferenceException(nameof(_webPubSubServiceClient));
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(webSocketUri, CancellationToken.None);
            var rcvBytes = new byte[128];
            var rcvBuffer = new ArraySegment<byte>(rcvBytes);
            var cts = new CancellationTokenSource();
            //Pas bo
            await Task.Factory.StartNew(
                async () =>
                {
                    while (true)
                    {
                        WebSocketReceiveResult rcvResult = await clientWebSocket.ReceiveAsync(rcvBuffer, CancellationToken.None);
                        byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                        string rcvMsg = Encoding.UTF8.GetString(msgBytes);
                        
                        Response(rcvMsg);
                    }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);




            Wait(MESSAGE);
            await _webPubSubServiceClient.CloseAllConnectionsAsync();
        }
        public async Task Stop()
        {
            if (_webPubSubServiceClient != null)
            {
                await _webPubSubServiceClient.CloseAllConnectionsAsync();
            }
            
        }
    }
    
}