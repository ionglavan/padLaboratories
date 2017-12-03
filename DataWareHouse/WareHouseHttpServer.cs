using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataWareHouse
{
    class WareHouseHttpServer
    {
        private Object lockThis = new Object();
        static CassandraService cassandraService;
        String cassandraTable;
        public WareHouseHttpServer(int portInt)
        {
            string url = "http://localhost";
            string port = portInt + "";
            string prefix = String.Format("{0}:{1}/", url, port);
            if (portInt == 8001)
            {
                cassandraTable = "employee1";
            }
            else
            {
                cassandraTable = "employee2";
            }
            cassandraService = new CassandraService(cassandraTable);

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

            Console.WriteLine(request.Url);
            switch (request.HttpMethod)
            {
                //create
                case "PUT":
                    PUT(context, requestBody);
                    break;

                //get
                case "GET":
                    GET(context);
                    break;

                //update
                case "POST":
                    POST(context, requestBody);
                    break;
            }
        }


        private static void PUT(HttpListenerContext context, String requestBody)
        {
            String succes = "Succes!";
            byte[] succesBytes = Encoding.ASCII.GetBytes(succes);
            using (Stream stream = context.Response.OutputStream)
            {
                stream.Write(succesBytes, 0, succesBytes.Length);
                stream.Close();
            }

            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);
            Console.WriteLine(requestBody);
            cassandraService.InsertEmployee(employee);

        }

        private static void POST(HttpListenerContext context, String requestBody)
        {

            String succes = "Succes!";
            byte[] succesBytes = Encoding.ASCII.GetBytes(succes);
            using (Stream stream = context.Response.OutputStream)
            {
                stream.Write(succesBytes, 0, succesBytes.Length);
                stream.Close();
            }

            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);
            cassandraService.UpdateEmployee(employee);
        }

        private static void GET(HttpListenerContext context)
        {
            List<Employee> employees = new List<Employee>();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Uri uri = request.Url;

            if (uri.ToString().Contains("allemployees"))
            {
                employees = cassandraService.GetEmployees();
            } else if (uri.ToString().Contains("employee"))
            {
                String id = HttpUtility.ParseQueryString(uri.Query).Get("id");
                employees.Add(cassandraService.GetEmployee(id));
            }


            var json = JsonConvert.SerializeObject(employees);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(json);

            response.ContentType = "text/json";
            response.StatusCode = (int)HttpStatusCode.OK;
            using (Stream stream = response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }


   
    }
}
