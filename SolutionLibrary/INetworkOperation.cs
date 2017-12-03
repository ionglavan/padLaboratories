﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConnectionPlatform
{
    public interface INetworkOperation
    {
        void Read();
        void Write(Message msg, IPEndPoint writeTo);
    }
}
