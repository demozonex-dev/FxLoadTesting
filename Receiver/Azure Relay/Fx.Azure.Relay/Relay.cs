using Microsoft.Azure.Relay;

namespace Fx.Receiver
{

    public class Relay : ReceiverBase, IReceiver
    {
        
        HybridConnectionListener? _listener;
        private string _relaynamespace;
        private string _hybridconnection;
        private string _saskeyname;
        private string _key;
        public string ReceiverType { get; }
        const string MESSAGE = "Connected to Azure Relay HybridConnection, waiting for message";
        public Relay(string relaynamespace, string hybridconnection, string saskeyname, string key)
        {
            if (relaynamespace == null) { throw new ArgumentNullException(nameof(relaynamespace)); }
            if (hybridconnection == null) { throw new ArgumentNullException(nameof(hybridconnection)); }
            if (saskeyname == null) { throw new ArgumentNullException(nameof(saskeyname)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            ReceiverType = "Relay";

            _relaynamespace= relaynamespace;
            _hybridconnection= hybridconnection;
            _saskeyname= saskeyname;
            _key= key;

        }
        

        public async Task StartAsync()
        {
            

            if (Wait == null) throw new NullReferenceException(nameof(Wait));
            if (Response == null) throw new NullReferenceException(nameof(Response));
            
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_saskeyname, _key);
            
            _listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}.servicebus.windows.net/{1}", 
                                                     _relaynamespace, 
                                                     _hybridconnection)), 
                                                     tokenProvider);            
            

            _listener.RequestHandler = (context) =>
            {
                ProcessEventGridEvents(context);
                context.Response.StatusCode = System.Net.HttpStatusCode.OK;
                context.Response.Close();
            };

            await _listener.OpenAsync();
            Wait(MESSAGE);
            await _listener.CloseAsync();
        }
        private void ProcessEventGridEvents(RelayedHttpListenerContext context)
        {            
            string message = new StreamReader(context.Request.InputStream).ReadToEnd();
            if(Response != null)
            {
                Response(message);
            }            
        }
        public async Task StopAsync()
        {
            if (_listener != null)
            {
                await _listener.CloseAsync();
            }
        }

      
    }
  
}