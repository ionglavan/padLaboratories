using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Node 
{
    class Node
    {

        static int port;
        static String name;
        static List<int> connectionPorts;
        static List<int> onlineConnectionPorts = new List<int>();
        static List<Employee> employees = new List<Employee>();
        static List<Employee> nodeEmployees = new List<Employee>();
        static bool discover = true;
        static int connections = 0;
        static Object thisLock = new Object();

        public Node(String name, int port, List<int> connectionPorts, List<Employee> employees)
        {

            Node.port = port;
            Node.name = name;
            Node.connectionPorts = connectionPorts;
            Node.nodeEmployees.AddRange(employees);


            Thread discoveryListener = new Thread(Listen);
            discoveryListener.Start();

            Task tk = new Task(MyFuncton);
            tk.Start();
            tk.Wait();
        }

        static async void MyFuncton()
        {
            
        }


        public static void Proc(Object objects)
        {
            byte[] buffer = new byte[1024];
            ((Socket)objects).Receive(buffer);
            Object obj = XmlOperations.ByteArrayToObject(buffer);
            try
            {
                Request req = (Request)obj;
                if (req.requestType.Equals("fromClient"))
                {
                    Console.WriteLine("Action: " + req.action);
                    
                    employees.Clear();
                    employees.AddRange(nodeEmployees);
                    Request newReq = new Request("employeeList", "", port);
                    foreach (int portVar in onlineConnectionPorts)
                    {
                        Write(newReq, portVar);
                    }


                    switch (req.action)
                    {
                        case "filter":
                            Timer myTimer = new Timer(Filter, req.value, 2000, Timeout.Infinite);
                            break;
                        case "sort":
                            Timer myTimer2 = new Timer(Sort, null, 2000, Timeout.Infinite);
                            break;
                        case "group":
                            Timer myTimer3 = new Timer(Group, null, 2000, Timeout.Infinite);
                            break;
                    }

                }

                if (req.requestType.Equals("employeeList"))
                {
                    Console.WriteLine("request employee list");
                    Write(nodeEmployees, req.value);
                }

            }
            catch (System.InvalidCastException) { }

            try
            {
                List<Employee> empl = (List<Employee>)obj;
                lock (thisLock) 
                { 
                employees.AddRange(empl);
                Console.WriteLine("Eployees received, total:" + employees.Count);
                }
            }
            catch (System.InvalidCastException) { }

        }




        static void Listen()
        {
            var listener = new UdpClient(port);
            listener.Client.ReceiveTimeout = 2000;
            var groupEP = new IPEndPoint(IPAddress.Any, port);
            string message;
            // Console.WriteLine("Waiting...");

            while (discover)
            {
                try
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    if (message.Equals("startDiscovery"))
                    {
                        Console.WriteLine("\n Start discovery");
                        MulticastRequest();
                        Timer myTimer = new Timer(EndDiscovery, null, 3000, Timeout.Infinite);

                    }
                    else
                    {
                        int nodePort = Int32.Parse(message);
                        foreach (int p in connectionPorts)
                        {
                            if (nodePort == p)
                            {
                                onlineConnectionPorts.Add(nodePort);
                                Console.WriteLine(name + " received message from {0}\n", message);
                                connections++;
                            }
                        }
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                }
            }


        }

        static void MulticastRequest()
        {
            byte[] data = Encoding.UTF8.GetBytes("" + port);
            IPEndPoint sending_end_point;
            var client = new UdpClient();

            for (int portVar = 8001; portVar <= 8080; portVar++)
            {
                sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portVar);
                client.Connect(sending_end_point);
                // send data
                client.Send(data, data.Length);

            }
        }

        static void SendPortAndNodes(int connections)
        {
            byte[] data = Encoding.UTF8.GetBytes(port + ":" + connections);
            IPEndPoint sending_end_point;
            var client = new UdpClient();
            sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            client.Connect(sending_end_point);
            // send data
            client.Send(data, data.Length);

        }





        public static void Write(Object obj, int port)
        {
            byte[] bytes = XmlOperations.ObjectToByteArray(obj);
            var tcpClient = new TcpClient("127.0.0.1", port);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        static void Filter(Object obj)
        {
            int minSalary = (int)obj;
            List<Employee> list2 = new List<Employee>();
            foreach (var item in employees)
            {
                if (item.salary > minSalary)
                {
                    list2.Add(item);
                }
            }

            Write(list2, 9000);

        }

        static void Group(Object obj)
        {
            List<Employee> listEmployees = new List<Employee>();
            List<IGrouping<String, Employee>> list2 = employees.GroupBy(o => o.department).ToList();
            foreach (var item in list2)
            {
                List<Employee> tempList = item.ToList();
                listEmployees.AddRange(tempList);
            }
            Write(listEmployees, 9000);
        }

        static void Sort(Object obj)
        {
            List<Employee> list3 = employees.OrderBy(o => o.lastName).ToList();
            Write(list3, 9000);
        }

        static void EndDiscovery(Object obj)
        {
            discover = false;

            Console.WriteLine("Connections on " + name + ": " + connections);
            SendPortAndNodes(connections);


            TcpListener listener = new TcpListener(port);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);

            }
        }

    }
}
