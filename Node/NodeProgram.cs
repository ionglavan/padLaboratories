using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node
{
    class NodeProgram
    {
        static void Main(string[] args)
        {
            List<Employee> employees = new List<Employee>();

            Console.WriteLine("Node nr: ");
            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    employees.Add(new Employee(1, "radu", "zmeu", "faf", 500));
                    employees.Add(new Employee(2, "plot", "qwer", "faf", 600));
                    break;
                case '2':
                    employees.Add(new Employee(3, "ion", "doarme", "fi", 300));
                    employees.Add(new Employee(4, "sasa", "zereghici", "fi", 700));
                    break;
            }
            EmployeeList employeeList = new EmployeeList(employees);
            string xml = ObjectOperations.Serialize(employeeList);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);
            using (var client = new System.Net.WebClient())
            {
                byte[] responseBytes = client.UploadData("http://localhost:8000", "PUT", bytes);
                Console.WriteLine(Encoding.ASCII.GetString(responseBytes));
            }
            Console.ReadLine();

        }
    }
}
