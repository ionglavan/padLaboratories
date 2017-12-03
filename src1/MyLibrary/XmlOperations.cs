using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyLibrary
{
    public class XmlOperations
    {
        public static String Serialize(Object obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }


        public static EmployeeList DeserializeEmployeeList(String str)
        {
            var xmlSerializer = new XmlSerializer(typeof(EmployeeList));
            var stringReader = new StringReader(str);
            return (EmployeeList)xmlSerializer.Deserialize(stringReader);
        }

        public static Request DeserializeRequest(String str)
        {
            var xmlSerializer = new XmlSerializer(typeof(Request));
            var stringReader = new StringReader(str);
            return (Request)xmlSerializer.Deserialize(stringReader);
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
