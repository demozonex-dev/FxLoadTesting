using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Relay;
using Azure.ResourceManager.Relay.Models;

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

        public async Task EasyInitAsync(string resourcegroupname, bool interactive=true)
        {
            if (resourcegroupname == null) { throw new ArgumentNullException(nameof(resourcegroupname)); }
            if (interactive)
            {
                
                
                Login(new InteractiveBrowserCredential());

                InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions();
                //options.TokenCachePersistenceOptions = new TokenCachePersistenceOptions();
                //Login(new InteractiveBrowserCredential(options));

            }
            else
            {
                Login(new VisualStudioCredential());
            }


            await SetDefaultSubscriptionAsync();
            await SetDefaultResourceGroupAsync(resourcegroupname);

        }

    }  
}