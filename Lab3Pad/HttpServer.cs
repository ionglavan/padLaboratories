using MyLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Lab5Pad
{
    class HttpServer
    {
        static List<Employee> employees = new List<Employee>();
        private Object lockThis = new Object();
        public HttpServer()
        {
            string url = "http://localhost";
            string port = "8000";
            string prefix = String.Format("{0}:{1}/", url, port);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            Console.WriteLine("Listening on {0}...", prefix);
            while (true)
            {
                ThreadPool.QueueUserWorkItem(Process, listener.GetContext());
            }
        }

        void Process(object o)
        {
            //Ожидание входящего запроса
            HttpListenerContext context = (HttpListenerContext)o;
            //Объект запроса
            HttpListenerRequest request = context.Request;
            //Объект ответа
            HttpListenerResponse response = context.Response;

            //Создаем ответ
            string requestBody;
            Stream inputStream = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(inputStream, encoding);
            requestBody = reader.ReadToEnd();


            switch (request.HttpMethod)
            {
                case "PUT":
                    EmployeeList empls = ObjectOperations.DeserializeEmployeeList(requestBody);
                    lock (lockThis)
                    {
                        employees.AddRange(empls.employees);
                        Console.WriteLine("Eployees received: " + empls.employees.Count + ", total: " + employees.Count);
                    }
                    String succes = "Succes!";
                    byte[] succesBytes = Encoding.ASCII.GetBytes(succes);
                    using (Stream stream = response.OutputStream)
                    {
                        stream.Write(succesBytes, 0, succesBytes.Length);
                        stream.Close();
                    }
                    break;

                case "GET":
                    Console.WriteLine(request.Url);
                    lock (lockThis)
                    {
                        byte[] bytes = PerformRequest(request.Url);

                        response.ContentType = "text/xml";
                        response.StatusCode = (int)HttpStatusCode.OK;
                        using (Stream stream = response.OutputStream)
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Close();
                        }
                    }           
                    break;
            }
        }

        static byte[] PerformRequest(Uri url)
        {
            String id = "", limit = "", offset = "";
            int idInt = 0, limitInt = 0, offsetInt = 0;
            List<Employee> returnList = new List<Employee>();

            if (url.ToString().Contains("/employee/"))
            {
                id = HttpUtility.ParseQueryString(url.Query).Get("id");
                idInt = Int32.Parse(id);
                foreach (Employee emp in employees)
                {
                    if (emp.id == idInt)
                    {
                        returnList.Add(emp);
                    }
                }
            }

            if (url.ToString().Contains("/employees/"))
            {
                limit = HttpUtility.ParseQueryString(url.Query).Get("limit");
                offset = HttpUtility.ParseQueryString(url.Query).Get("offset");
                limitInt = Int32.Parse(limit);
                offsetInt = Int32.Parse(offset);
                for (int i = 0; i < employees.Count; i++)
                {
                    try
                    {
                        if(employees[i].id >= offsetInt && employees[i].id <= limitInt)
                        returnList.Add(employees[i]);
                    }
                    catch (Exception) { }
                }     
            }

            if (url.ToString().Contains("/allemployees/"))
            {
                returnList.AddRange(employees);
            }
    
            EmployeeList eml = new EmployeeList(returnList);
            String xml = ObjectOperations.Serialize(eml);
            return System.Text.Encoding.ASCII.GetBytes(xml);
        }


    }
}
