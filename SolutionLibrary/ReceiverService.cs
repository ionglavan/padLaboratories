using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionPlatform
{
    public class ReceiverService : INetworkOperation
    {
        static IPEndPoint broker;
        static IPEndPoint receiver;

        public ReceiverService(IPEndPoint broker, IPEndPoint receiver)
        {
            ReceiverService.broker = broker;
            ReceiverService.receiver = receiver;
        }

        public void Read()
        {
            Thread t = new Thread(ReadInAnotherThread);
            t.Start();
        }

        static void ReadInAnotherThread()
        {
            TcpListener listener = new TcpListener(receiver);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);
            }
        }


        public static void Proc(object socket)
        {
            byte[] buffer = new byte[1024];
            ((Socket)socket).Receive(buffer);
            XmlOperations xmlOperation = new XmlOperations();
            Message msg = (Message)xmlOperation.ByteArrayToObject(buffer);
            String message = xmlOperation.Serialize(msg);
            if(xmlOperation.XmlValidation(message) == true)
                Console.WriteLine(xmlOperation.ParseMessage(message));
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
