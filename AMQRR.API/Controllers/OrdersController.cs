using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using AMQRR.API.Services.Singleton;
using AMQRR.Common.Configuration;
using AMQRR.Common.Models;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;


namespace AMQRR.API.Controllers
{
    public class OrdersController : ApiController
    {
        private IMqService _mqService;
        private const int HTTP_TIMEOUT_SECONDS = 20; // TODO: Use HttpRuntime.ExecutionTimout

        // default ctor
        public OrdersController()
        {
            _mqService = Services.Singleton.MqService.GetInstance();
        }

        // di ctor
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
            throw new NotImplementedException();
        }

        // POST: api/Orders
        public Order Post([FromBody] Order order)
        {
            var queue = Queue.API_ORDERS_POST;
            var timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECONDS);
            var correlationId = Request.GetCorrelationId().ToString();

            IDestination requestQueue = SessionUtil.GetDestination(_mqService.Session, queue);
            IDestination responseQueue = _mqService.Session.CreateTemporaryQueue();
            
            using (var producer = _mqService.MqSession.Session.CreateProducer(requestQueue))
            {
                string serialized = JsonConvert.SerializeObject(order);
                ITextMessage message = _mqService.Session.CreateTextMessage(serialized);
                message.NMSTimeToLive = timeout;
                message.NMSReplyTo = responseQueue;
                message.NMSCorrelationID = correlationId;

                producer.Send(message);
            }
            
            using (var consumer = _mqService.Session.CreateConsumer(responseQueue))
            {
                ITextMessage message = (ITextMessage) consumer.Receive(timeout);
                if (message == null) throw new NullReferenceException("Received blank message.");

                var serialized = message.Text;

                if (message.NMSCorrelationID != correlationId)
                {
                    throw new InvalidOperationException("CorrelationId Mismatch. Aborting.");
                }
                else
                {
                    return JsonConvert.DeserializeObject<Order>(serialized);
                }
            }

            throw new NotImplementedException();
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
