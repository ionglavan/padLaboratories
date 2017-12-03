using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLibrary;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Node
{ 
    public class NodeTransportService
    {
        int mainNodePort;
        String nodeIpAddress = "127.0.0.1";
        public NodeTransportService(int port)
        {
            this.mainNodePort = port;

            Thread t = new Thread(() => ReadInAnotherThread(port));
            t.Start();
        }

        static void ReadInAnotherThread(int port)
        {
            TcpListener listener = new TcpListener(port);
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
            Request request = (Request)XmlOperations.ByteArrayToObject(buffer);
            Console.WriteLine();
        }


        public void Write(EmployeeList employeeListObject)
        {
            byte[] bytes = XmlOperations.ObjectToByteArray(employeeListObject);
            var tcpClient = new TcpClient(nodeIpAddress, mainNodePort);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
