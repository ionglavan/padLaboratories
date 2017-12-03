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
            Console.WriteLine("1. Filter by salary");
            Console.WriteLine("2. Sort by last name");
            Console.WriteLine("3. Group by department");
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
                        Console.Write(" Enter minimal salary: ");
                        String minimalSalary = Console.ReadLine();
                        transportService.FilterBySalary(Int32.Parse(minimalSalary));
                        break;

                    case '2':
                        if (mainNodePort == 0) break;
                        transportService.SortByLastName();
                        break;

                    case '3':
                        if (mainNodePort == 0) break;
                        transportService.GroupByDepartment();
                        break;
                }
            }
        }
    }
}
