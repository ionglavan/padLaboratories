using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class Employee
    {
        [System.Xml.Serialization.XmlElement("id")]
        public int id;

        [System.Xml.Serialization.XmlElement("firstName")]
        public String firstName;

        [System.Xml.Serialization.XmlElement("lastName")]
        public String lastName;

        [System.Xml.Serialization.XmlElement("department")]
        public String department;

        [System.Xml.Serialization.XmlElement("salary")]
        public float salary;

        public Employee(int id, String firstName, String lastName, String department, float salary)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.department = department;
            this.salary = salary;
        }

        public Employee()
        {

        }
    }
}
