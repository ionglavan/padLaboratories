using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ConnectionPlatform;
using System.Net;
using System.Threading;

namespace Sender
{
    class SenderProgram
    {
        static void Main(string[] args)
        {
            //Connect to broker and search for messages
            IPEndPoint broker = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            INetworkOperation senderService = new SenderService(broker, sender);
            XmlOperations xmlOperation = new XmlOperations();
            senderService.Read();
            


            while (true)
            {
                Console.WriteLine("1. Write message to receiver 1");
                Console.WriteLine("2. Write message to receiver 2");
                Console.WriteLine("3. Write message to all receivers");

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        XmlDocument doc = new XmlDocument();
                        doc.Load(@"D:\receiver1.xml");
                        Message msg1 = xmlOperation.Deserialize(doc.OuterXml);
                        senderService.Write(msg1, broker);
                        break;

                    case '2':
                        XmlDocument doc1 = new XmlDocument();
                        doc1.Load(@"D:\receiver2.xml");
                        Message msg2 = xmlOperation.Deserialize(doc1.OuterXml);
                        senderService.Write(msg2, broker);
                        break;

                    case '3':
                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(@"D:\allReceivers.xml");
                        Message msg3 = xmlOperation.Deserialize(doc2.OuterXml);
                        senderService.Write(msg3, broker);
                        break;
                }
                Console.Clear();
            }



        }
    }
}
