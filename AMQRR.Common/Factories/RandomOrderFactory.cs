using AMQRR.Common.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMQRR.Common.Factories
{
    public class RandomOrderFactory
    {
        private Random _random;
        private string[] _productNames;

        private int _nextOrderId = 0;
        private int _nextOrderLineId = 0;

        public RandomOrderFactory()
        {
            _random = new Random();
            _productNames = new String[] { "Toy Car", "Toy Dinosaur", "Toy Soldier", "Toy Tank" };
        }

        public Order Create()
        {
            Order order = CreeateOrder();
            List<OrderLine> lines = CreateOrderLines(order.OrderId, _random.Next(1,3));

            order.OrderLines = lines;

            return order;
        }

        private List<OrderLine> CreateOrderLines(int orderId, int count)
        {
            var orderLines = new List<OrderLine>();

            for(var lineSeq =1; lineSeq<count; lineSeq++)
            {
                var orderLine = new OrderLine
                {
                    OrderLineId = GetNextOrderLineId(),
                    OrderId = orderId,
                    Sequence = lineSeq,
                    ProductName = _productNames[_random.Next(_productNames.Length)],
                    UnitPrice = _random.Next(10, 100) / 4,
                    Quantity = _random.Next(1,7)
                };
                orderLines.Add(orderLine);
            };

            return orderLines;
        }

        private Order CreeateOrder()
        {
            var order = new Order
            {
                OrderId = GetNextOrderId(),
                Customer = _random.Next(1000, 9999)
            };

            return order;
        }

        private int GetNextOrderId()
        {
            _nextOrderId += 1;
            return _nextOrderId;
        }

        private int GetNextOrderLineId()
        {
            _nextOrderLineId += 1;
            return _nextOrderLineId;
        }
    }
}
