using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLibrary;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client
{
    public class ClientTransportService
    {
        int mainNodePort;
        String nodeIpAddress = "127.0.0.1";
        public ClientTransportService(int mainNodePort)
        {
            this.mainNodePort = mainNodePort;

            Thread t = new Thread(ReadInAnotherThread);
            t.Start();
        }

        static void ReadInAnotherThread()
        {
            TcpListener listener = new TcpListener(9000);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);
            }
        }

            public static void Proc(object socket)
        {
            Console.WriteLine("\n");
            byte[] buffer = new byte[1024];
            ((Socket)socket).Receive(buffer);
            List<Employee> employeeList = (List<Employee>)XmlOperations.ByteArrayToObject(buffer);
                foreach(var item in employeeList) {
                    Console.WriteLine("First name " + item.firstName);
                    Console.WriteLine("Last name " + item.lastName);
                    Console.WriteLine("Salary " + item.salary);
                    Console.WriteLine("Department " + item.department + "\n");
                }
            Console.WriteLine("done");
        }


        public void FilterBySalary(int minSalary) 
        {
            Request r = new Request("fromClient", "filter", minSalary);
            Write(r);
        }

        public void SortByLastName() 
        {
            Request r = new Request("fromClient", "sort", 0);
            Write(r);
        }

        public void GroupByDepartment()
        {
            Request r = new Request("fromClient", "group", 0);
            Write(r);
        }

        public void Write(Request requestObject)
        {
            byte[] bytes = XmlOperations.ObjectToByteArray(requestObject);
            var tcpClient = new TcpClient(nodeIpAddress, mainNodePort);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
