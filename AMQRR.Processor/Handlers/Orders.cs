using AMQRR.Common.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMQRR.Common.Models;
using Apache.NMS;
using Newtonsoft.Json;

namespace AMQRR.Processor.Handlers
{
    public class Orders
    {

        private const int HTTP_TIMEOUT_SECONDS = 20;

        private MqSession _mqSession;

        public Orders(MqSession mqSession)
        {
            _mqSession = mqSession;
        }

        public void OnOrderPostReceived(IMessage message)
        {
            var timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECONDS);
            var textMessage = (ITextMessage)message;
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
