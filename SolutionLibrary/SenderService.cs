using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConnectionPlatform
{
    public class SenderService : INetworkOperation
    {
        static IPEndPoint broker;
        static IPEndPoint sender;

        public SenderService(IPEndPoint broker, IPEndPoint sender)
        {
            SenderService.broker = broker;
            SenderService.sender = sender;
        }

        public void Read()
        {

            Thread t = new Thread(ReadInAnotherThread);
            t.Start();

        }

        static void ReadInAnotherThread()
        {
                        TcpListener listener = new TcpListener(sender);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);
            }
        }

        public void Write(Message msg, IPEndPoint writeTo)
        {
            XmlOperations xmlOperation = new XmlOperations();
            byte[] bytes = xmlOperation.ObjectToByteArray(msg);
            var tcpClient = new TcpClient(writeTo.Address.ToString(), writeTo.Port);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Proc(object socket)
        {
            byte[] buffer = new byte[1024];
            ((Socket)socket).Receive(buffer);
            INetworkOperation senderService = new SenderService(broker, sender);
            XmlOperations xmlOperation = new XmlOperations();
            Message msgObject = (Message)xmlOperation.ByteArrayToObject(buffer);
            String message = xmlOperation.Serialize(msgObject);
            Console.WriteLine(xmlOperation.ParseMessage(message));
            switch (xmlOperation.ParseDestination(message))
            {
                case "receiver1":
                    XmlDocument doc1 = new XmlDocument();
                    doc1.Load(@"D:\receiver1.xml");
                    Message msg1 = xmlOperation.Deserialize(doc1.OuterXml);
                    senderService.Write(msg1, broker);
                    break;
                case "receiver2":
                    XmlDocument doc2 = new XmlDocument();
                    doc2.Load(@"D:\receiver2.xml");
                    Message msg2 = xmlOperation.Deserialize(doc2.OuterXml);
                    senderService.Write(msg2, broker);
                    break;
            }       
        }
    }
}
