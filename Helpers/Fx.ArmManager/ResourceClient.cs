using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.EventGrid;
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

        public void Login(TokenCredential tokencredential)
        {
            if (tokencredential == null) { throw new ArgumentNullException(nameof(tokencredential)); }
            Client = new ArmClient(tokencredential);
            if (Client == null) { throw new NullReferenceException(nameof(Client)); }         
        }


        public SubscriptionResource SetDefaultSubscription()
        {
            if (Client == null) { throw new NullReferenceException(nameof(Client)); }

            return _sub = Client.GetDefaultSubscription(); ;

        }

        public ResourceGroupResource SetDefaultResourceGroup(string resourcegroupname)
        {
            if (string.IsNullOrEmpty(resourcegroupname)) { throw new ArgumentNullException(nameof(resourcegroupname)); }
            if (_sub == null) { throw new NullReferenceException(nameof(_sub)); }
            return _group = _sub.GetResourceGroup(resourcegroupname);
        }
        public async Task<(Uri endpoint, string key)> GetEventGridConnectionInfos(string topicname)
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



    }  
}