namespace TestNetworkInfo
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Get_HostName()
        {
            
            string hostName=NetworkInfo.GetHostName();
            Assert.Pass(hostName);
            
        }
        [Test]
        public void Test_Get_IpAddresses()
        {

            string hostName = NetworkInfo.GetHostName();
            if (hostName == null)
            {
                Assert.Fail("Host name is null");
            }
            else
            {
                string ip = NetworkInfo.GetLocalIPV4Address(hostName);
                if (ip == null)
                {
                    Assert.Fail("IP address is null");
                }
                Assert.Pass(ip);
            }
            

        }
    }
}