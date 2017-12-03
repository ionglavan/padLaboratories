using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConnectionPlatform
{
    public class BrokerService : INetworkOperation
    {
        static IPEndPoint broker;
        static IPEndPoint receiver1, receiver2;
        static IPEndPoint sender;
        static XmlDocument brokerXmlDoc = new XmlDocument();
            
        public BrokerService(IPEndPoint broker, IPEndPoint receiver1, IPEndPoint receiver2, IPEndPoint sender)
        {
            BrokerService.broker = broker;
            BrokerService.receiver1 = receiver1;
            BrokerService.receiver2 = receiver2;
            BrokerService.sender = sender;         
        }

        public void WriteToBrokerXml(Message message, Boolean status)
        {
            lock (brokerXmlDoc)
            {
                XmlDocument doc = new XmlDocument();
                XmlOperations xmlOperations = new XmlOperations();
                String msgString = xmlOperations.Serialize(message);
                doc.LoadXml(msgString);

                XmlNode rootNode = brokerXmlDoc.DocumentElement;
                XmlNode newElem = brokerXmlDoc.CreateNode("element", "messageItem", "");
                XmlNode statusNode = brokerXmlDoc.CreateNode("element", "status", "");

                statusNode.InnerText = status.ToString();
                newElem.AppendChild(statusNode);
    
                XmlNode messageNode = doc.DocumentElement;
                newElem.AppendChild(brokerXmlDoc.ImportNode(doc.DocumentElement, true));
                rootNode.AppendChild(newElem);
                brokerXmlDoc.Save(@"D:/broker.xml");
            }
        }


        public void Read()
        {
            
            TcpListener listener = new TcpListener(8888);
            listener.Start();
            brokerXmlDoc.Load(@"D:/broker.xml"); 
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);
            }
        }

        void WriteMessage(IPEndPoint address, Message message, BrokerService bService)
        {
            try
            {
                bService.Write(message, address);
                WriteToBrokerXml(message, true);  
            }
            catch (Exception)
            {
                WriteToBrokerXml(message, false);
            }
        }

        public static void Proc(object socket)
        {
            byte[] buffer = new byte[1024];
            ((Socket)socket).Receive(buffer);
            //String message = ASCIIEncoding.ASCII.GetString(buffer);

            XmlOperations xmlOperation = new XmlOperations();
            Message msgObject = (Message)xmlOperation.ByteArrayToObject(buffer);
            msgObject.data = DateTime.Now.ToString();
            String message = xmlOperation.Serialize(msgObject);
            Console.WriteLine(message);
            BrokerService bService = new BrokerService(broker, receiver1, receiver2, sender);
            switch (xmlOperation.ParseAction(message))
            {
                case "connect":
                    //find not delivered messages
                    String sendTo = xmlOperation.ParseDestination(message);
                    Console.WriteLine("connect user: " + sendTo);
                    
                    lock (brokerXmlDoc)
                    {
                        XElement root = XElement.Parse(brokerXmlDoc.OuterXml);
                        IEnumerable<XElement> messages =
                            from el in root.Elements("messageItem")
                            where (string)el.Element("status") == "False"
                            select el;
                        foreach (XElement el in messages)
                        {
                            String to = el.Element("properties").Element("to").Value;
                            if (sendTo == to)
                            {
                                switch (sendTo)
                                {
                                    case "receiver1":
                                        String msg = el.Element("properties").ToString();
                                        Message m = xmlOperation.Deserialize(msg);
                                        Console.WriteLine(msg);
                                        try
                                        {
                                            bService.Write(m, receiver1);
                                            el.Element("status").Value = "True";
                                        } catch(Exception) {}
                                        break;

                                    case "receiver2":
                                        String msg2 = el.Element("properties").ToString();
                                        Message m2 = xmlOperation.Deserialize(msg2);
                                        Console.WriteLine(msg2);
                                        try
                                        {
                                            bService.Write(m2, receiver2);
                                            el.Element("status").Value = "True";
                                        } catch(Exception) {}
                                        break;
                                }
                               
                            }
                            Console.WriteLine(to);
                        }

                        brokerXmlDoc.LoadXml(root.ToString());
                        brokerXmlDoc.Save(@"D:/broker.xml");
                            
                    }
                    break;

                case "receiver":
                    switch (xmlOperation.ParseDestination(message))
                    {
                        case "receiver1":
                            //send message to receiver 1
                            Console.WriteLine("write to receiver 1");
                            bService.WriteMessage(receiver1, msgObject, bService);
                            break;
                        case "receiver2":
                            //send message to receiver 1
                            Console.WriteLine("write to receiver 2");
                            Message msgr2 = xmlOperation.Deserialize(message);
                            bService.WriteMessage(receiver2, msgObject, bService);
                            break;
                    }
                    break;
                case "all":
                    Console.WriteLine("write to all receivers");
                    msgObject.to = "receiver1";
                    bService.WriteMessage(receiver1, msgObject, bService);
                    msgObject.to = "receiver2";
                    bService.WriteMessage(receiver2, msgObject, bService);
                    break;
                case "request":
                    Console.WriteLine("request from the sender");
                    bService.Write(msgObject, sender);
                    break;
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
    }
}
