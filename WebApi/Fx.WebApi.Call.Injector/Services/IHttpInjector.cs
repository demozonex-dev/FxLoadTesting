namespace Fx.WebApi.Call.Injector.Services
{
    public interface IHttpInjector
    {
        Task<string> Headers();
        Task<HttpResponseMessage> EvengridInjector();

    }
}
