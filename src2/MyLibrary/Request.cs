using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Request")]
    public class Request
    {
        public Request()
        {

        }

            [System.Xml.Serialization.XmlElement("requestType")]
            public String requestType;

            [System.Xml.Serialization.XmlElement("action")]
            public String action;

            [System.Xml.Serialization.XmlElement("value")]
            public int value;

            public Request(String requestType, String action, int value)
            {
                this.action = action;
                this.value = value;
                this.requestType = requestType;
            }
    }
}
