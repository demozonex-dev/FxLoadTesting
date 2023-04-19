using Azure.Core;
using Azure.Identity;
using Fx.ArmManager;
using Fx.Injector;

namespace Fx.WebApi.Injector.Services
{
    public class EventGridInjectorService
    {
        private ILogger<EventGridInjectorService> _logger;
        private IConfiguration _configuration;
        public IInjector Injector { get; set; }
        public EventGridInjectorService(ILogger<EventGridInjectorService> logger,
                                        IConfiguration configuration) 
        { 
            _logger = logger;
            _configuration = configuration;
            Injector = CreateEventGridInjector().Result;
        }
        async Task<IInjector> CreateEventGridInjector()
        {

            IConfigurationSection? parameterSection = _configuration.GetSection("parameters");
            if (parameterSection == null) { throw new NullReferenceException(nameof(parameterSection)); }
            ResourceClient resourceClient = new ResourceClient();
            string? resourceGroup = parameterSection["resourcegroup:value"];
            if (resourceGroup == null) { throw new NullReferenceException($"{nameof(resourceGroup)}"); }

            TokenCredential? tokenCredential = null;
#if (_DEBUG)

            tokenCredential = new VisualStudioCredential();

#else
            //Need to RBAC to the ressources for the User Managed Identity
            string clientId = parameterSection["managedidentity:clientid"];
            tokenCredential=new ManagedIdentityCredential(clientId);
#endif
            await resourceClient.EasyInitAsync(resourceGroup, tokenCredential);
            string? topicName = parameterSection["eventgridtopic:value"];
            if (topicName == null) { throw new NullReferenceException(nameof(topicName)); }
            var eventGridConnectionInfos = await resourceClient.GetEventGridConnectionInfosAsync(topicName);
            return new Fx.Injector.EventGrid(eventGridConnectionInfos.endpoint, eventGridConnectionInfos.key);

        }
    }
}
