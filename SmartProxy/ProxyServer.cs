using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartProxy
{
    class ProxyServer
    {
        private Object lockThis = new Object();
        private static RedisService redisService = new RedisService();
        public ProxyServer()
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
            String requestBody;
            Stream inputStream = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(inputStream, encoding);
            requestBody = reader.ReadToEnd();

            Console.WriteLine(requestBody);
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
            //random warehouse
            String url = context.Request.Url.ToString();
            String newUrl = "";
            switch (LoadBalancingStrategy())
            {
                case 1:
                    newUrl = ReplaceFirst(url, "8000", "8001");
                    break;
                case 2:
                    newUrl = ReplaceFirst(url, "8000", "8002");
                    break;
            }
            //

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestBody);
            using (var client = new System.Net.WebClient())
            {
                client.UploadData(newUrl, "PUT", bytes);
            }

             String succes = "Succes!";
             byte[] succesBytes = Encoding.ASCII.GetBytes(succes);
             using (Stream stream = context.Response.OutputStream)
             {
                 stream.Write(succesBytes, 0, succesBytes.Length);
                 stream.Close();
             }
             
        }

        private static void POST(HttpListenerContext context, String requestBody)
        {
            //random warehouse
            String url = context.Request.Url.ToString();
            String newUrl = "";
            switch (LoadBalancingStrategy())
            {
                case 1:
                    newUrl = ReplaceFirst(url, "8000", "8001");
                    break;
                case 2:
                    newUrl = ReplaceFirst(url, "8000", "8002");
                    break;
            }
            //

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestBody);
            using (var client = new System.Net.WebClient())
            {
                client.UploadData(newUrl, "POST", bytes);
            }

            String succes = "Succes!";
            byte[] succesBytes = Encoding.ASCII.GetBytes(succes);
            using (Stream stream = context.Response.OutputStream)
            {
                stream.Write(succesBytes, 0, succesBytes.Length);
                stream.Close();
            }
        }

        private static void GET(HttpListenerContext context)
        {
            String url = context.Request.Url.ToString();
            String redisValue = redisService.RedisRead(url);
            if (redisValue != null)
            {
                HttpListenerResponse response1 = context.Response;
                byte[] bytes1 = System.Text.Encoding.ASCII.GetBytes(redisValue);
                response1.ContentType = "text/json";
                response1.StatusCode = (int)HttpStatusCode.OK;
                using (Stream stream = response1.OutputStream)
                {
                    stream.Write(bytes1, 0, bytes1.Length);
                    stream.Close();
                }
                Console.WriteLine(redisValue);
                return;
            }

            //random warehouse
            String newUrl = "";
            switch (LoadBalancingStrategy())
            {
                case 1:
                    newUrl = ReplaceFirst(url, "8000", "8001");
                    break;
                case 2:
                    newUrl = ReplaceFirst(url, "8000", "8002");
                    break;
            }
            //

            WebRequest newRequest = WebRequest.Create(newUrl);
            WebResponse newResponse = newRequest.GetResponse();
            Stream inputStream = newResponse.GetResponseStream();
            StreamReader reader = new StreamReader(inputStream);
            String responseBody = reader.ReadToEnd();
            reader.Close();
            inputStream.Close();
            Console.WriteLine(responseBody);
            //

            HttpListenerResponse response = context.Response;
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(responseBody);
            response.ContentType = "text/json";
            response.StatusCode = (int)HttpStatusCode.OK;
            using (Stream stream = response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            redisService.RedisWrite(url, responseBody);
        }


        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static int LoadBalancingStrategy()
        {
            Random random = new Random();
            return random.Next(1, 3);
        }

    }
}
