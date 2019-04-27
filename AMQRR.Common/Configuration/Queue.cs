using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQRR.Common.Configuration
{
    public class Queue
    {
        public const string API_ORDERS_POST = "api.orders.post";
        public const string API_ORDERS_POST_REPLY = API_ORDERS_POST + ".reply";

        public const string API_ORDERS_GET = "api.orders.get";
        public const string API_ORDERS_GET_REPLY = API_ORDERS_GET + ".reply";
    }
}
