
using System.Net;

namespace Fx.Helpers
{
    public class NetworkInfo
    {
        // Create a methode to get the IP address of the local machine
        public static string GetLocalIPV4Address(string hostname)
        {
            // Return the IP address
            IPAddress[] addresses = Dns.GetHostEntry(hostname).AddressList;
            
            //Create a loop to go through the IP address
            string ip = null;
            foreach (IPAddress address in addresses)
            {
                
                // If the address is IPv4
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    // Return the address
                    ip= address.ToString();
                    continue;
                }
            }

            return ip;
        }
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
    }
    
}