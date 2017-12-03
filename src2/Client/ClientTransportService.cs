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
        static List<Employee> employees;
        int mainNodePort;
        String nodeIpAddress = "127.0.0.1";
        public ClientTransportService(int mainNodePort)
        {
            this.mainNodePort = mainNodePort;
            employees = new List<Employee>();

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
            byte[] buffer = new byte[2048];
            ((Socket)socket).Receive(buffer);
            String message = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            EmployeeList employeeList = (EmployeeList)XmlOperations.DeserializeEmployeeList(message);
            Console.WriteLine("List of employee\n");
            PrintListEmployees(employeeList.employees);
            employees.Clear();
            employees.AddRange(employeeList.employees);
            Console.WriteLine("END");
        }

        public static void PrintListEmployees(List<Employee> empl)
        {
            foreach (var item in empl)
            {
                Console.WriteLine("First name " + item.firstName);
                Console.WriteLine("Last name " + item.lastName);
                Console.WriteLine("Salary " + item.salary);
                Console.WriteLine("Department " + item.department + "\n");
            }
        }

        public void GetData()
        {
            Request r = new Request("fromClient", "getData", 0);
            Write(r);
        }

        public void FilterBySalary(int minSalary)
        {
            List<Employee> list = new List<Employee>();
            foreach (var item in employees)
            {
                if (item.salary > minSalary)
                {
                    list.Add(item);
                }
            }
            PrintListEmployees(list);
        }

        public void SortByLastName()
        {
            List<Employee> list = employees.OrderBy(o => o.lastName).ToList();
            PrintListEmployees(list);
        }

        public void GroupByDepartment()
        {
            List<Employee> listEmployees = new List<Employee>();
            List<IGrouping<String, Employee>> list2 = employees.GroupBy(o => o.department).ToList();
            foreach (var item in list2)
            {
                List<Employee> tempList = item.ToList();
                listEmployees.AddRange(tempList);
            }
            PrintListEmployees(listEmployees);
        }

        public void Write(Request requestObject)
        {
            String message = XmlOperations.Serialize(requestObject);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            var tcpClient = new TcpClient(nodeIpAddress, mainNodePort);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
