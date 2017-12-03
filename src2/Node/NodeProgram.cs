using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Node
{
    class NodeProgram
    {
        static void Main(string[] args)
        {
            Console.Write("Enter node number: ");
            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    List<int> connectionPorts = new List<int>();
            connectionPorts.Add(8002);
            List<Employee> employees = new List<Employee>();
            employees.Add(new Employee("Radu", "Zmeu","dep1", 1000));
            employees.Add(new Employee("Ion", "Caraian", "dep2", 2000));
            Node node1 = new Node("Node1", 8001, connectionPorts, employees);
                    break;

                case '2':
                    List<int> connectionPorts2 = new List<int>();
                    connectionPorts2.Add(8001);
                    connectionPorts2.Add(8003);
                    connectionPorts2.Add(8005);
            List<Employee> employees2 = new List<Employee>();
            employees2.Add(new Employee("Vasia", "L1", "dep2", 500));
            employees2.Add(new Employee("Sasa", "L2", "dep3", 20000));
            Node node2 = new Node("Node2", 8002, connectionPorts2, employees2);
                    break;

                case '3':
                    List<int> connectionPorts3 = new List<int>();
                    connectionPorts3.Add(8002);
                    connectionPorts3.Add(8004);
            List<Employee> employees3 = new List<Employee>();
            employees3.Add(new Employee("Stefan", "L2", "fi", 9000));
            employees3.Add(new Employee("Nicu", "L3", "dep2", 200));
            Node node3 = new Node("Node3", 8003, connectionPorts3, employees3);
                    break;
                case '4':
                    List<int> connectionPorts4 = new List<int>();
            connectionPorts4.Add(8003);
            connectionPorts4.Add(8005);
            List<Employee> employees4 = new List<Employee>();
            employees4.Add(new Employee("F1", "L4", "fi", 3000));
            employees4.Add(new Employee("F2", "L1", "dep1", 2500));
            Node node4 = new Node("Node4", 8004, connectionPorts4, employees4);
                    break;
                case '5':
                    List<int> connectionPorts5 = new List<int>();
            connectionPorts5.Add(8002);
            connectionPorts5.Add(8004);
            List<Employee> employees5 = new List<Employee>();
            employees5.Add(new Employee("F2", "L5", "dep4", 500));
            employees5.Add(new Employee("F2", "L1", "dep4", 900));
            Node node5 = new Node("Node5", 8005, connectionPorts5, employees5);
                    break;
                case '6':
                    List<int> connectionPorts6 = new List<int>();
            List<Employee> employees6 = new List<Employee>();
            employees6.Add(new Employee("F3", "L2", "fi", 100));
            employees6.Add(new Employee("F4", "L3", "dep1", 20));
            Node node6 = new Node("Node6", 8006, connectionPorts6, employees6);
                    break;
            }

            Console.ReadLine();

        }
       
    }
}
