using Microsoft.Extensions.Configuration;

namespace Fx.Injector
{
    public interface IInjector
    {
        
        public Task SendAsync(object message);
        public string  InjectorType { get; }

    }
}