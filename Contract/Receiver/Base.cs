using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{
    public class ReceiverBase
    {
              
        public Action<string> Wait { get; set; }
        public Action<string> Response { get; set; }
        
        
    }
}
