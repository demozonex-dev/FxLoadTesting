using Microsoft.Extensions.Configuration;

namespace Fx.Receiver
{
    public class Base
    {
              
        public Action<string> Wait { get; set; }
        public Action<string> Response { get; set; }
        
        
    }
}
