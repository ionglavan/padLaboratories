using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionPlatform
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("properties")]
    public class Message
    {
        [System.Xml.Serialization.XmlElement("action")]
        public String action;
        [System.Xml.Serialization.XmlElement("to")]
        public String to;
        [System.Xml.Serialization.XmlElement("message")]
        public String message;
        [System.Xml.Serialization.XmlElement("data")]
        public String data;
    }
}
