using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.Relay;
using Azure.ResourceManager.Relay.Models;
using Azure.ResourceManager.Resources;

namespace Fx.ArmManager
{
    public enum Resource 
    {

    }
    public class ResourceClient
    {
        public ArmClient Client { get; set; }
        //private ArmClient? _client;
        private SubscriptionResource? _sub;
        private ResourceGroupResource? _group;
        private RelayNamespaceResource? _relay;
        private RelayHybridConnectionResource? _hybridConnection;

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
            return _group = await _sub.GetResourceGroupAsync(resourcegroupname);
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

        public async Task<string> GetRelayKeyAsync(string relaynamespace,string hybridconnection, string saskeyname)
        {
            if (relaynamespace==null) { throw  new ArgumentNullException(nameof(relaynamespace)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }

            RelayNamespaceResource relay=  await _group.GetRelayNamespaceAsync(relaynamespace);
            //RelayHybridConnectionResource hcr= await relay.GetRelayHybridConnectionAsync(hybridconnection);
            RelayNamespaceAuthorizationRuleResource rules = await relay.GetRelayNamespaceAuthorizationRuleAsync(saskeyname);

            RelayAccessKeys keys =await rules.GetKeysAsync();

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

        public async Task<EventGridTopicResource> CreateOrUpdateEventGridTopicAsync(
                                                            string eventgridname, 
                                                            string eventsubscriptionname,
                                                            string location)
        {
            if (eventgridname == null) { { throw new ArgumentNullException(nameof(eventgridname)); } }
            if (eventgridname == null) { throw new ArgumentNullException(nameof(eventgridname)); }
            if (_group == null) { throw new NullReferenceException(nameof(_group)); }
            
            if (_hybridConnection== null) { throw new NullReferenceException(nameof(_hybridConnection)); }

            AzureLocation azurelocation = new AzureLocation(location);

            var armOperation = await _group.GetEventGridTopics()
                         .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                              eventgridname,
                                              new EventGridTopicData(azurelocation));
            EventGridSubscriptionData subscriptionData = new EventGridSubscriptionData();
            

            await armOperation.Value.GetTopicEventSubscriptions()
                                        .CreateOrUpdateAsync(Azure.WaitUntil.Completed,
                                        eventsubscriptionname,
                                        new EventGridSubscriptionData { 
                                        Destination=new HybridConnectionEventSubscriptionDestination
                                            {
                                               ResourceId=_hybridConnection.Id,
                                            }
                                        });
                                                             

                                        

            return armOperation.Value;
            
        }

        public async Task<RelayNamespaceResource> CreateOrUpdateRelayNamespaceAsync(string relaynamespace,
                                                                                    string hybridconnection,
                                                                                    string location)
                                                                                    
        {
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
                                                                  
    }  
}