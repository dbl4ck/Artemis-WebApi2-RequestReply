using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using AMQRR.API.Base;
using AMQRR.API.Services.Singleton;
using AMQRR.Common.Configuration;
using AMQRR.Common.Models;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;


namespace AMQRR.API.Controllers
{
    public class OrdersController : MqController
    {
        private IMqService _mqService;
      
        // default ctor
        public OrdersController()
        {
            _mqService = Services.Singleton.MqService.GetInstance();
        }

        // dependency injection ctor
        public OrdersController(IMqService mqService)
        {
            _mqService = mqService;
        }

        // GET: api/Orders
        public IEnumerable<Order> Get()
        {
            throw new NotImplementedException();
        }

        // GET: api/Orders/5
        public Order Get(int id)
        {
            var queueName = Queue.API_ORDERS_GET;
            var replyQueueName = Queue.API_ORDERS_GET_REPLY;

            var data = id.ToString();

            var result = ExecuteRequestReply(data, queueName, replyQueueName, CorrelationId);

            return JsonConvert.DeserializeObject<Order>(result);
        }

        // POST: api/Orders
        public Order Post([FromBody] Order order)
        {
            var queueName = Queue.API_ORDERS_POST;
            var replyQueueName = Queue.API_ORDERS_POST_REPLY;

            var data = JsonConvert.SerializeObject(order);

            var result = ExecuteRequestReply(data, queueName, replyQueueName, CorrelationId);

            return JsonConvert.DeserializeObject<Order>(result);
        }

        

        // PUT: api/Orders/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Orders/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
