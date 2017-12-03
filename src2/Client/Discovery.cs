using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Discovery
    {
        static bool discover = true;

        static void EndDyscovery(Object obj) {
            Console.WriteLine("Dyscovery ended.");
            discover = false;
        }
        public Discovery()
        {
            Timer myTimer = new Timer(EndDyscovery, null, 5000, Timeout.Infinite);
            //Thread t = new Thread(EndDyscovery);
            //t.Start();
        }

        public int GetNodePort()
        {
            var listener = new UdpClient(9000);
            listener.Client.ReceiveTimeout = 2000000;
            var groupEP = new IPEndPoint(IPAddress.Any, 9000);
            string message;
            Console.WriteLine("Waiting...");
            List<String> nodesList = new List<String>();
            int mainNodePort = 0;
            int temp = -1;
            while (discover)
            {
                try
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            
                    nodesList.Add(message);
                    listener.Client.ReceiveTimeout = 2000;
                }
                catch (System.Net.Sockets.SocketException){ }
            }
            listener.Close();
                    foreach (var str in nodesList)
                    {
                        String[] mes = str.Split(':');
                        Console.WriteLine("port: " + mes[0] + " connections: " + mes[1]);
                        if (temp < Int32.Parse(mes[1]))
                        {
                            temp = Int32.Parse(mes[1]);
                            mainNodePort = Int32.Parse(mes[0]);
                        }
                    }
                    
                    return mainNodePort;
                
            }

        public void MulticastRequest()
        {
            byte[] data = Encoding.UTF8.GetBytes("startDiscovery");
            IPEndPoint sending_end_point;
            var client = new UdpClient();

            for (int portVar = 8001; portVar <= 8010; portVar++)
            {
                sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portVar);
                client.Connect(sending_end_point);
                // send data
                client.Send(data, data.Length);

            }
        }
        
    }
}
