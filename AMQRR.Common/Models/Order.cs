using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQRR.Common.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int Customer{ get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}
