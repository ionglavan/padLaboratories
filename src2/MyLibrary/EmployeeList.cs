using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyLibrary
{
    [Serializable()]
    [XmlRoot("EmployeeList")]
    [XmlInclude(typeof(Employee))] // include type class Person
    public class EmployeeList
    {
    [XmlArray("EmployeeArray")]
    [XmlArrayItem("EmployeeObject")]
    public List<Employee> employees = new List<Employee>();

    public EmployeeList()
    {

    }
    }
}
