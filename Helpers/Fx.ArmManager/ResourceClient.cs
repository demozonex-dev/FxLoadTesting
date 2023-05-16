using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.Relay;
using Azure.ResourceManager.Relay.Models;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.ServiceBus;
using Azure.ResourceManager.ServiceBus.Models;
using Azure.ResourceManager.AppService.Models;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.ResourceManager.WebPubSub;

namespace Fx.ArmManager
{
    public enum Resource 
    {

    }
    public class ResourceClient
    {
        public ArmClient? Client { get; set; }
        
        private SubscriptionResource? _sub;
        private ResourceGroupResource? _group;
        private RelayNamespaceResource? _relay;
        private RelayHybridConnectionResource? _hybridConnection;
        private ServiceBusQueueResource? _serviceBusQueue;

        public void Login(TokenCredential tokencredential)
        {
            if (tokencredential == null) { throw new ArgumentNullException(nameof(tokencredential)); }
            Client = new ArmClient(tokencredential);
            if (Client == null) { throw new NullReferenceException(nameof(Client)); }         
        }


        public async Task<SubscriptionResource> SetDefaultSubscriptionAsync()
        {
            if (Client == null) { throw new NullReferenceException(nameof(Client)); }

            return _sub = await Client.GetDefaultSubscriptionAsync(); ;

        }

        public async Task<ResourceGroupResource> SetDefaultResourceGroupAsync(string resourcegroupname)
        {
            if (string.IsNullOrEmpty(resourcegroupname)) { throw new ArgumentNullException(nameof(resourcegroupname)); }
            if (_sub == null) { throw new NullReferenceException(nameof(_sub)); }
            _group = await _sub.GetResourceGroupAsync(resourcegroupname);
            
            return _group;
        }
        public async Task<(Uri endpoint, string key)> GetEventGridConnectionInfosAsync(string topicname)
        {
            
            if (string.IsNullOrEmpty(topicname)) { throw new ArgumentNullException(nameof(topicname)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            string? key = null;
            Uri? endPointUri = null;
            
          
            EventGridTopicResource topic = await _group.GetEventGridTopicAsync(topicname);
            if (topic == null) { throw new NullReferenceException(nameof(topicname)); }
            endPointUri = topic.Data.Endpoint;
            TopicSharedAccessKeys keys = await topic.GetSharedAccessKeysAsync();
            if (keys == null) { throw new NullReferenceException(nameof(keys)); }
            key = keys.Key1;        
            return new(endPointUri, key);          
          
        }
        public async Task<string> GetWebPubSubConnectionStringAsync(string wepubsubname)
        {
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }
            if (wepubsubname == null) { throw new ArgumentNullException(nameof(wepubsubname)); }
            var webPubSubHub=await _group.GetWebPubSubAsync(wepubsubname);
            
            if (webPubSubHub == null) { throw new NullReferenceException(nameof(webPubSubHub)); }
            var key=await webPubSubHub.Value.GetKeysAsync();
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            return key.Value.PrimaryConnectionString;
            
        }
        public async Task<string> GetServiceBusConnectionStringAsync(string servicebusnamespace,
                                                                     string saskeyname)
        {
            
            string key = await GetServiceBusKeyAsync(servicebusnamespace, saskeyname);
            return  $"Endpoint=sb://{servicebusnamespace}.servicebus.windows.net/;SharedAccessKeyName={saskeyname};SharedAccessKey={key}";
        }
        private  async Task<string> GetServiceBusKeyAsync(string servicebusnamespace, 
                                                         string saskeyname )
        {
            if (servicebusnamespace == null) { throw new ArgumentNullException(nameof(servicebusnamespace)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            ServiceBusNamespaceResource serviceBus= await _group.GetServiceBusNamespaceAsync(servicebusnamespace);
            ServiceBusNamespaceAuthorizationRuleResource rule= await serviceBus.GetServiceBusNamespaceAuthorizationRuleAsync(saskeyname);
            
            
            ServiceBusAccessKeys keys=await rule.GetKeysAsync();
            if (keys == null) { throw new NullReferenceException(nameof(keys)); }
            return keys.PrimaryKey;
            
        }
        public async Task<string> GetRelayKeyAsync(string relaynamespace,string hybridconnection, string saskeyname)
        {
            if (relaynamespace==null) { throw  new ArgumentNullException(nameof(relaynamespace)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            RelayNamespaceResource relay=  await _group.GetRelayNamespaceAsync(relaynamespace);
            //RelayHybridConnectionResource hcr= await relay.GetRelayHybridConnectionAsync(hybridconnection);
            RelayNamespaceAuthorizationRuleResource rule = await relay.GetRelayNamespaceAuthorizationRuleAsync(saskeyname);

            RelayAccessKeys keys =await rule.GetKeysAsync();

            return keys.PrimaryKey;
        }

        public async Task EasyInitAsync(string resourcegroupname, 
                                        TokenCredential tokenCredential)
        {
            if (resourcegroupname == null) { throw new ArgumentNullException(nameof(resourcegroupname)); }

            Login(tokenCredential);


            await SetDefaultSubscriptionAsync();
            await SetDefaultResourceGroupAsync(resourcegroupname);

        }

        public async Task CreateOrUpdateAppService(string name, AppServiceSkuDescription sku, string kind, string location)
        {
            if (name == null) { throw new ArgumentNullException(nameof (name)); }
            if (location == null) { throw new ArgumentNullException (nameof (location)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            AppServicePlanData data = new AppServicePlanData(new AzureLocation(location));
            data.Sku = sku;
            data.Kind = kind;
            var op= await _group.GetAppServicePlans().CreateOrUpdateAsync(Azure.WaitUntil.Completed,name, data); 
        }
        public async Task<ResourceGroupResource> CreateOrUpdateResourceGroupAsync(string resourcegroupname,
                                                                                  string location)
        {
            if (resourcegroupname == null) { throw new ArgumentNullException(nameof(resourcegroupname)); }

            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (_sub == null) { throw new NullReferenceException(nameof(_sub)); }
            var armOperation= await _sub.GetResourceGroups()
                                        .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                                             resourcegroupname,
                                                             new ResourceGroupData(location));
            return _group=armOperation.Value;
            
        }

        public async Task<EventGridTopicResource> CreateOrUpdateEventGridTopicAsync(string eventgridname, 
                                                                                    string eventsubscriptionname,
                                                                                    string location)
        {
            if (eventgridname == null) { { throw new ArgumentNullException(nameof(eventgridname)); } }
            if (eventgridname == null) { throw new ArgumentNullException(nameof(eventgridname)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }
            
            if (_hybridConnection== null) { throw new NullReferenceException(nameof(_hybridConnection)); }
            if (_serviceBusQueue==null) {  throw new NullReferenceException(nameof(_serviceBusQueue)); }

            AzureLocation azurelocation = new AzureLocation(location);

            var armOperation = await _group.GetEventGridTopics()
                         .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                              eventgridname,
                                              new EventGridTopicData(azurelocation));
            EventGridSubscriptionData subscriptionData = new EventGridSubscriptionData();
            

            await armOperation.Value.GetTopicEventSubscriptions()
                                        .CreateOrUpdateAsync(Azure.WaitUntil.Started,
                                        $"{eventsubscriptionname}-relay",
                                        new EventGridSubscriptionData { 
                                        Destination=new HybridConnectionEventSubscriptionDestination
                                            {
                                               ResourceId=_hybridConnection.Id,
                                            }
                                        });


            await armOperation.Value.GetTopicEventSubscriptions()
                                        .CreateOrUpdateAsync(Azure.WaitUntil.Started,
                                        $"{eventsubscriptionname}-sb",
                                        new EventGridSubscriptionData
                                        {
                                            Destination = new ServiceBusQueueEventSubscriptionDestination 
                                            {
                                                ResourceId = _serviceBusQueue.Id,
                                            }
                                        });

            return armOperation.Value;
            
        }

        public async Task<RelayNamespaceResource> CreateOrUpdateRelayNamespaceAsync(string relaynamespace,
                                                                                    string hybridconnection,
                                                                                    string location)
                                                                                    
        {
            if (location ==null) { throw new ArgumentNullException(nameof(location)); }
            if (relaynamespace == null) { throw new ArgumentNullException(nameof(relaynamespace)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            AzureLocation azurelocation = new AzureLocation(location);
            var armOperation = await _group.GetRelayNamespaces()
                         .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                              relaynamespace,
                                              new RelayNamespaceData(azurelocation));
            var op=await armOperation.Value.GetRelayHybridConnections()
                                           .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                                         hybridconnection, 
                                                         new RelayHybridConnectionData
                                                         {
                                                             IsClientAuthorizationRequired = true,
                                                         });
            _hybridConnection = op.Value;

            return armOperation.Value;
        }
        
        public async Task CreateOrUpdateServiceBusAsync(string servicebusname,                                                        
                                                        string queue,
                                                        string topic,
                                                        string location)
        {
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (servicebusname == null) { throw new ArgumentNullException(nameof(servicebusname)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            var armOperation = await _group.GetServiceBusNamespaces()
                                                 .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                                                      servicebusname,
                                                                      new ServiceBusNamespaceData(new AzureLocation(location)));


            var opQueue= await armOperation.Value.GetServiceBusQueues()
                                .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                                    queue,
                                                    new ServiceBusQueueData
                                                    {
                                                        
                                                    });

            _serviceBusQueue=opQueue.Value;

            await armOperation.Value.GetServiceBusTopics()
                                    .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                                         topic,
                                                         new ServiceBusTopicData
                                                         {
                                                             
                                                         });
        }
    }  
}