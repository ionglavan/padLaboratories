using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartProxy
{
    public class RedisService
    {

        public String RedisRead(String request)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            String value = cache.StringGet(request);
            return value;
        }

        public void RedisWrite(String request, String response)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            TimeSpan timeSpan = new TimeSpan(0,0,5);// 5 sec
            cache.StringSet(request, response, timeSpan);
        }
    }
}
