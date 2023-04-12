using Azure.ResourceManager.Resources;
using Azure.ResourceManager.ServiceBus;
using Azure.ResourceManager.EventGrid;

using Fx.ArmManager;
using Azure.ResourceManager.EventGrid.Models;
using Azure.Identity;

namespace TestResourceManager
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test_Get_EventGrid_Keys()
        {
            var arm = new ResourceClient();
            arm.Login(new VisualStudioCredential());
            var sub = await arm.Client.GetDefaultSubscriptionAsync();
            string resourceGroupName = "cna100-rg";
            
            ResourceGroupResource resourceGroup=sub.GetResourceGroup(resourceGroupName);
            string topicName = "grideventgrid100";
            EventGridTopicResource topic= await resourceGroup.GetEventGridTopicAsync(topicName);
            Console.WriteLine(topic.Data.Endpoint.ToString());
            TopicSharedAccessKeys keys=await topic.GetSharedAccessKeysAsync();
            
            Console.WriteLine(keys.Key1);
            Assert.Pass();
        }
        [Test]
        public async Task Test_GetRelayInfo()
        {
            ResourceClient resourceClient = new ResourceClient();
            resourceClient.Login(new VisualStudioCredential());
            resourceClient.SetDefaultSubscriptionAsync();
            resourceClient.SetDefaultResourceGroupAsync("cna100-rg");

            
            string? resourceGroupName = "cna100-rg";
            

            string hybridConnection = "hcdeployment";
            string relayNameSpace = "gridrelay100";

            string sasKeyName = "RootManageSharedAccessKey";
            string key = await resourceClient.GetRelayKeyAsync(relayNameSpace, hybridConnection, sasKeyName); 
            Console.WriteLine($"{key}");
            Assert.Pass();

        }
    }
}