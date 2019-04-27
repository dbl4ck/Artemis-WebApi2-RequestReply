using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQRR.Common.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int OrderId { get; set; }
        public int Sequence { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
