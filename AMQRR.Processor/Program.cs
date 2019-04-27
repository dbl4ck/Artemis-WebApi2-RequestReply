using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMQRR.Common;
using AMQRR.Common;
using AMQRR.Common.Configuration;
using AMQRR.Common.Models;
using AMQRR.Common.MQ;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;

namespace AMQRR.Processor
{
    class Program
    {
        private const int HTTP_TIMEOUT_SECONDS = 20;
        private static int _sleepMs = 10;
        private static MqSession _mqSession;

        static void Main(string[] args)
        {
            _mqSession = new MqSession(Url.BROKER_URL);
            
            // setup order-post listener
            var orderPostQueue = SessionUtil.GetDestination(_mqSession.Session, Queue.API_ORDERS_POST);
            var orderPostConsumer = _mqSession.Session.CreateConsumer(orderPostQueue);
            orderPostConsumer.Listener += OnOrderPostReceived;

            // main loop
            while (true)
            {
                Thread.Sleep(_sleepMs);
            }
        }
        
        // Handlers
        private static void OnOrderPostReceived(IMessage message)
        {
            var timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECONDS);
            var textMessage = (ITextMessage) message;
            var replyQueue = textMessage.NMSReplyTo;
            var correlationId = textMessage.NMSCorrelationID;

            var order = JsonConvert.DeserializeObject<Order>(textMessage.Text);

            Console.WriteLine($"OrderId={order.OrderId}, Customer={order.Customer}");

            using (var producer = _mqSession.Session.CreateProducer(replyQueue))
            {
                var serialized = JsonConvert.SerializeObject(order);
                var replyMessage = _mqSession.Session.CreateTextMessage(textMessage.Text);
                replyMessage.NMSCorrelationID = correlationId;
                replyMessage.NMSTimeToLive = timeout;

                producer.Send(replyMessage);
            }

        }
    }
}
