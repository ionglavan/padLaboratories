using Cassandra;
using MyLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataWareHouse
{
    class CassandraService
    {

        ISession session;
        String cassandraTable;
        public CassandraService(String cassandraTable)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            session = cluster.Connect("keyspace1");
            this.cassandraTable = cassandraTable;
            //
            this.cassandraTable = "employee1";
        }

        public void UpdateEmployee(Employee employee)
        {
            session.Execute("insert into " + cassandraTable + " (id, first_name, last_name, salary," +
            "department) values (" + employee.id + ", '" + employee.firstName + "', '" +
            employee.lastName + "', " + employee.salary + ", '" + employee.department + "')");
        }

        public void InsertEmployee(Employee employee)
        {
            List<Employee> employees = GetEmployees();
            if (employees.Count > 0)
            {
                Employee lastEmployee = employees.OrderBy(o => o.id).ToList().Last();
                employee.id = lastEmployee.id + 1;
            }
            else
            {
                employee.id = 1;
            }
            session.Execute("insert into " + cassandraTable + " (id, first_name, last_name, salary," +
            "department) values (" + employee.id + ", '" + employee.firstName + "', '" +
            employee.lastName + "', " + employee.salary + ", '" + employee.department + "')");
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            RowSet result = session.Execute("select * from " + cassandraTable);
            foreach (Row row in result)
            {
                Employee empl = new Employee();
                empl.id = row.GetValue<Int32>("id");
                empl.firstName = row.GetValue<String>("first_name");
                empl.lastName = row.GetValue<String>("last_name");
                empl.department = row.GetValue<String>("department");
                empl.salary = row.GetValue<float>("salary");
                employees.Add(empl);
            }
            Console.WriteLine("Get employees, count: " + employees.Count);
            return employees;
        }

        public Employee GetEmployee(String id)
        {
            Employee empl = new Employee();
            try
            {
                Row row = session.Execute("select * from " + cassandraTable + " where id=" + id).First();
                empl.id = row.GetValue<Int32>("id");
                empl.firstName = row.GetValue<String>("first_name");
                empl.lastName = row.GetValue<String>("last_name");
                empl.department = row.GetValue<String>("department");
                empl.salary = row.GetValue<float>("salary");
            }
            catch (Exception) { }
            return empl;
        }

    }
}
