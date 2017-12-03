using ConnectionPlatform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ConnectionPlatform
{
    public class XmlOperations : IXmlOperations
    {
        string xsdMarkup =
@"<xs:schema attributeFormDefault='unqualified' elementFormDefault='qualified' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xs:element name='properties'>
    <xs:complexType>
      <xs:sequence>
        <xs:element type='xs:string' name='action'/>
        <xs:element type='xs:string' name='to'/>
        <xs:element type='xs:string' name='message'/>
        <xs:element type='xs:string' name='data'/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";

        public String Read(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc.OuterXml;
        }


        public String ParseAction(String xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList list = xmlDoc.GetElementsByTagName("action");
            return list[0].InnerText;
        }

        public String ParseMessage(String xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList list = xmlDoc.GetElementsByTagName("message");
            return list[0].InnerText;
        }

        public String ParseDestination(String xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList list = xmlDoc.GetElementsByTagName("to");
            return list[0].InnerText;
        }

        public Boolean XmlValidation(String xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XmlReader.Create(new StringReader(xsdMarkup)));
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

        public String AddXmlNode(String xml, String data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode properties = doc.SelectSingleNode("properties");
            XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "data", null);
            newNode.InnerText = data;
            properties.AppendChild(newNode);
            return doc.OuterXml;
        }

        public String SetDestination(String xml, String destination)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList list = xmlDoc.GetElementsByTagName("to");
            list[0].InnerText = destination;
            return xmlDoc.OuterXml;
        }

        public String Serialize(Message obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }


        public Message Deserialize(String str)
        {
            var xmlSerializer = new XmlSerializer(typeof(Message));
            var stringReader = new StringReader(str);
            return (Message)xmlSerializer.Deserialize(stringReader);
        }

        public byte[] ObjectToByteArray(Object obj)
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

        public Object ByteArrayToObject(byte[] arrBytes)
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
