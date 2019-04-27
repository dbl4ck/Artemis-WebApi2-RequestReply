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
using AMQRR.Processor.Handlers;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;

namespace AMQRR.Processor
{
    class Program
    {
        private static int _sleepMs = 10;
        private static MqSession _mqSession;

        static void Main(string[] args)
        {
            Console.Title = "Processor";
            Console.WindowWidth = 100;

            _mqSession = new MqSession(Url.BROKER_URL);
            
            // setup listeners
            CreateConsumer(Queue.API_ORDERS_POST).Listener += new Orders(_mqSession).OnOrderPostReceived;
            CreateConsumer(Queue.API_ORDERS_GET).Listener += new Orders(_mqSession).OnOrderGetReceived;

            // main loop
            while (true)
            {
                Thread.Sleep(_sleepMs);
            }
        }

        private static IMessageConsumer CreateConsumer(string queueName)
        {
            var queue = SessionUtil.GetDestination(_mqSession.Session, queueName);
            var consumer = _mqSession.Session.CreateConsumer(queue);
            return consumer;
        }
    }
}
