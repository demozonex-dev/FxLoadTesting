using Microsoft.Extensions.Configuration;

namespace Fx.Injector
{
    public interface IInjector
    {
        
        public Task Send(object message);
        
    }
}