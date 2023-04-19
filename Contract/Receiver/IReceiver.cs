using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{

    public interface IReceiver
    {
        
        public Action<string> Response { get; set; }
        public Action<string> Wait { get; set; }
        public Task StartAsync();
        public Task Stop();
        public string ReceiverType { get;  }


    }
}