using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AMQRR.Common.Models;
using Apache.NMS;
using Apache.NMS.Util;


namespace AMQRR.API.Controllers
{
    public class OrdersController : ApiController
    {
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
        public void Post([FromBody] Order order)
        {
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
