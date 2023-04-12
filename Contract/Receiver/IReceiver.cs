using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{

    public interface IReceiver
    {
        
        public Action<string> ReturnMessage { get; set; }
        public Action<string> Wait { get; set; }
        public Task Start(
                          /*Action<string> wait, 
                          Action<string> returnmessage*/);
        public Task Stop();
        
        
    }
}