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
        public async Task Test_GetEventGridInfo()
        {
            string? key = null;
            Uri? endPointUri = null;

            key = System.Environment.GetEnvironmentVariable("EVENT_GRID_KEYS");
            var uri= System.Environment.GetEnvironmentVariable("EVENT_GRID_END_POINT");


        }
    }
}