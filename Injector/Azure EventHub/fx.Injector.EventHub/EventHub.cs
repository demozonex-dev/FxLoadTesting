using Fx.Injector;

namespace fx.Injector
{
    public class EventHub : IInjector
    {
        public string InjectorType => throw new NotImplementedException();

        public Task SendAsync(object message)
        {
            throw new NotImplementedException();
        }
    }
}