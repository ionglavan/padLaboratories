using MyLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DataWareHouse
{
    class DataWareHouseService
    {
 
        public DataWareHouseService() 
        {
            TcpListener listener = new TcpListener(10000);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(Proc, socket);
            }
        }

        public static void Proc(object socket)
        {
            byte[] buffer = new byte[2048];
            ((Socket)socket).Receive(buffer);
            String message = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            EmployeeList employeeList = (EmployeeList)XmlOperations.DeserializeEmployeeList(message);
            //check data
            Console.WriteLine("check data");
            if (XmlValidation(message))
            {
                //write to client
                Write(employeeList, 9000);
            }  
        }

        public static void Write(EmployeeList obj, int port)
        {
            String message = XmlOperations.Serialize(obj);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            var tcpClient = new TcpClient("127.0.0.1", port);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        public static Boolean XmlValidation(String xml)
        {
            String xsdStr = System.IO.File.ReadAllText(@"F:\Dev\Lab4\schema0.xsd");
            xml = xml.Replace("\r\n", "");
            xml = xml.Replace("\0", "");
            XDocument doc = XDocument.Parse(xml);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XmlReader.Create(new StringReader(xsdStr)));
            String msg = "";
            doc.Validate(schemaSet, (o, e) =>
            {
                msg += e.Message + Environment.NewLine;
            });
            if (msg != "")
            {
                Console.WriteLine("Document invalid: " + msg);
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
