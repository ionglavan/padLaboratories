using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Discovery discovery = null;
            int mainNodePort = 0;

            ClientTransportService transportService = null;

            Console.WriteLine("0. Start discovery");
            Console.WriteLine("1. Get data");
            Console.WriteLine("2. Sort by last name");
            Console.WriteLine("3. Group by department");
            Console.WriteLine("4. Filter by min salary");
            while (true)
            {

                switch (Console.ReadKey().KeyChar)
                {
                    case '0':
                        if (discovery != null) break;
                        Console.Write(" \n Start discovery: ");
                        discovery = new Discovery();
                        discovery.MulticastRequest();
                        mainNodePort = discovery.GetNodePort();
                        transportService = new ClientTransportService(mainNodePort);
                        Console.WriteLine("Main node port: " + mainNodePort);
                        break;

                    case '1':
                        if (mainNodePort == 0) break;
                        Console.Write(" Get data\n");
                        transportService.GetData();
                        break;

                    case '2':
                        if (mainNodePort == 0) break;
                        Console.WriteLine("\n\n");
                        transportService.SortByLastName();
                        break;

                    case '3':
                        if (mainNodePort == 0) break;
                        Console.WriteLine("\n\n");
                        transportService.GroupByDepartment();
                        break;
                    case '4':
                        if (mainNodePort == 0) break;
                        Console.WriteLine("Enter min salary: ");
                        String minS = Console.ReadLine();

                        transportService.FilterBySalary(Int32.Parse(minS));
                        Console.WriteLine("\n\n");
                        break;
                }
            }
        }
    }
}
