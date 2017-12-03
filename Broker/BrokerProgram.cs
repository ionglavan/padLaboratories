using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionPlatform;
using System.Net;
using System.Threading;
namespace Broker
{
    class BrokerProgram
    {
        static void Main(string[] args)
        {
            //Wait and read messages from clients
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            IPEndPoint broker = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            IPEndPoint receiver1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9001);
            IPEndPoint receiver2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9002);
            INetworkOperation brokerService = new BrokerService(broker, receiver1, receiver2, sender);
            Boolean tryStart = true;
            while (tryStart)
            try
            {
                brokerService.Read();
                tryStart = false;
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
            }
            
        }
    }
}
