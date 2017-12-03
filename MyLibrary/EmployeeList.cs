using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyLibrary
{
    //[Serializable()]
    //[XmlRoot("EmployeeList")]
   // [XmlInclude(typeof(Employee))] // include type class Person
    public class EmployeeList
    {
        //[XmlArray("ArrayOfEmployee")]
        //[XmlArrayItem("Employee")]
        public List<Employee> employees;

        public EmployeeList(List<Employee> employees)
        {
            this.employees = employees;
        }

        public EmployeeList()
        {

        }
    }

}
