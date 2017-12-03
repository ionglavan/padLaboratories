using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionPlatform;
using System.Net;
namespace ReceiverOne
{
    class ReceiverProgram
    {
        static void Main(string[] args)
        {
            IPEndPoint broker = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9001);;
            ReceiverService receiverService = new ReceiverService(broker, receiver);
            receiverService.Read();
            XmlOperations xmlOperation = new XmlOperations();
            String message = xmlOperation.Read(@"D:\connectR1.xml");
            Message msg = xmlOperation.Deserialize(message);
            receiverService.Write(msg, broker);
            //request something frim the sender
            Console.WriteLine("Press enter to request something from the sender");
            while (true)
            {
                Console.ReadLine();
                String requestMessage = xmlOperation.Read(@"D:\request1.xml");
                Message msg1 = xmlOperation.Deserialize(requestMessage);
                receiverService.Write(msg1, broker);
            }
            
        }
    }
}
