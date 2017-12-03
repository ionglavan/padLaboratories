using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpClient
{
    class HttpService
    {

        public List<Employee> GetById(int id)
        {
            return GetEmployees("http://localhost:8000/employee/?id=" + id);
        }

        public List<Employee> GetAll()
        {
            return GetEmployees("http://localhost:8000/allemployees/");
        }

        public List<Employee> GetEmployees(String url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream inputStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(inputStream);
            String responseBody = reader.ReadToEnd();
            reader.Close();
            inputStream.Close();
            Console.WriteLine(responseBody);
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
            return employees;
        }

        public void AddEmployee(Employee employee)
        {
            string json = JsonConvert.SerializeObject(employee);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(json);
            using (var client = new System.Net.WebClient())
            {
                client.UploadData("http://localhost:8000", "PUT", bytes);
            }
        }

        public void ModifyEmployee(Employee employee)
        {
            string json = JsonConvert.SerializeObject(employee);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(json);
            using (var client = new System.Net.WebClient())
            {
                client.UploadData("http://localhost:8000", "POST", bytes);
            }
        }

    }
}
