using Microsoft.Azure.Relay;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Fx.Receiver
{
  
    public class Relay : Base, IReceiver
    {
        private readonly IConfiguration _configuration;
        HybridConnectionListener? _listener;
        //public IConfiguration? Configuration { get; set; }
        const string MESSAGE = "Connected to Azure Relay HybridConnection";
        public Relay(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        

        public async Task Start(/*Action<string> wait, Action<string> returnmessage*/)
        {

            if (Wait == null) throw new ArgumentNullException(nameof(Wait));
            if (ReturnMessage == null) throw new ArgumentNullException(nameof(ReturnMessage));
            if (_configuration == null) throw new ArgumentNullException(nameof(_configuration));

            string? SasKeyName = _configuration["SasKeyName"];
            string? Key = _configuration["Key"];
            string? RelayNameSpace = _configuration["RelayNameSpace"];
            string? HybridConnection = _configuration["HybridConnection"];
            

            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(SasKeyName, Key);
            
            _listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNameSpace, HybridConnection)), tokenProvider);            
            

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
            if(ReturnMessage != null)
            {
                ReturnMessage(message);
            }            
        }
        public async Task Stop()
        {
            if (_listener != null)
            {
                await _listener.CloseAsync();
            }
        }

      
    }
  
}