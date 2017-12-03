using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConnectionPlatform
{
        public interface IXmlOperations
        {
            String Read(String path);

            String ParseAction(String xml);

            String ParseMessage(String xml);

            String ParseDestination(String xml);

            Boolean XmlValidation(String xml);

            String AddXmlNode(String xml, String data);

            String SetDestination(String xml, String destination);

            String Serialize(Message xml);
            Message Deserialize(String str);
            byte[] ObjectToByteArray(Object obj);
            Object ByteArrayToObject(byte[] arrBytes);
        }
}
