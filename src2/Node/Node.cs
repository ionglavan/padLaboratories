using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Newtonsoft.Json;


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
        static Object lockThis = new Object();

        public Node(String name, int port, List<int> connectionPorts, List<Employee> employees)
        {

            Node.port = port;
            Node.name = name;
            Node.connectionPorts = connectionPorts;
            Node.nodeEmployees.AddRange(employees);


            Thread discoveryListener = new Thread(Listen);
            discoveryListener.Start();

        }


        public static void Proc(Object objects)
        {
            byte[] buffer = new byte[1024];
            ((Socket)objects).Receive(buffer);
            String message = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            Object obj;
            //if json
            if (message.StartsWith("{") || message.StartsWith("["))
            {
                try
                {
                    Request req = JsonConvert.DeserializeObject<Request>(message);
                    if (req.requestType.Equals("employeeList"))
                    {
                        Console.WriteLine("request employee list");
                        WriteJson(nodeEmployees, req.value);
                    }
                }
                catch (Exception) { }
               
                try
                {
                    lock (lockThis)
                    {
                        EmployeeList empl = JsonConvert.DeserializeObject<EmployeeList>(message);
                        employees.AddRange(empl.employees);
                        Console.WriteLine("Eployees received, total:" + employees.Count);
                    }
                }
                catch (System.Exception e) { }
            }
            else
            {
                obj = XmlOperations.DeserializeRequest(message);
                Request req = (Request)obj;
                if (req.requestType.Equals("fromClient"))
                {
                    Console.WriteLine("Action: " + req.action);

                    employees.Clear();
                    employees.AddRange(nodeEmployees);
                    Request newReq = new Request("employeeList", "", port);
                    foreach (int portVar in onlineConnectionPorts)
                    {
                        WriteJson(newReq, portVar);
                        Console.WriteLine("req from port: " + portVar);
                    }

                    //send data
                    Timer myTimer = new Timer(SendData, null, 3000, Timeout.Infinite);

                }
            }

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





        public static void WriteJson(Object obj, int port)
        {
            String jsonString = null;
            try
            {
                Request r = (Request)obj;
                jsonString = JsonConvert.SerializeObject(r);
            }
            catch (Exception) { }
            try
            {
                EmployeeList empList = new EmployeeList();
                empList.employees.AddRange((List<Employee>)obj);
                jsonString = JsonConvert.SerializeObject(empList);
            }
            catch (Exception) { }
            
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            var tcpClient = new TcpClient("127.0.0.1", port);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteXml(Object obj, int port)
        {
            EmployeeList empList = new EmployeeList();
            empList.employees.AddRange((List<Employee>)obj);
            var xmlString = XmlOperations.Serialize(empList);
            byte[] bytes = Encoding.UTF8.GetBytes(xmlString);
            var tcpClient = new TcpClient("127.0.0.1", port);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        static void SendData(Object obj)
        {
            WriteXml(employees, 10000);
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
