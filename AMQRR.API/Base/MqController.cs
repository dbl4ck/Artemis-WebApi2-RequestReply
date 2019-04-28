using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AMQRR.API.Services.Singleton;
using AMQRR.Common.Generators;
using AMQRR.Common.Enumerations;
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json;

namespace AMQRR.API.Base
{
    public abstract class MqController : ApiController
    {
        protected const int HTTP_TIMEOUT_SECONDS = 20; // TODO: Use HttpRuntime.ExecutionTimeout

        private const string ECorrelationIdMismatchAborting = "CorrelationId mismatch. Aborting.";
        private const string ENoMessageRequestReplyTimeout = "Received a null message from the consumer. This could be due to a request-reply timeout.";

        protected IMqService _mqService;

        // default ctor
        protected MqController()
        {
            _mqService = MqService.GetInstance();
        }

        // dependency injection ctor
        protected MqController(IMqService mqService)
        {
            _mqService = mqService;
        }

        protected string ExecuteRequestReply(string value, string queueName, string replyQueueName, string correlationId, int timeoutSecs = HTTP_TIMEOUT_SECONDS)
        {
            var timeout = TimeSpan.FromSeconds(timeoutSecs);
            var selector = new MqFilterGenerator().Add(NMSFilter.JMSCorrelationID, correlationId).ToString();

            IDestination requestQueue = SessionUtil.GetDestination(_mqService.Session, queueName);
            IDestination replyQueue = SessionUtil.GetDestination(_mqService.Session, replyQueueName);

            using (var producer = _mqService.MqSession.Session.CreateProducer(requestQueue))
            {
                //string serialized = JsonConvert.SerializeObject(value);
                ITextMessage message = _mqService.Session.CreateTextMessage(value);
                message.NMSTimeToLive = timeout;
                message.NMSReplyTo = replyQueue;
                message.NMSCorrelationID = correlationId;

                producer.Send(message);
            }

            using (var consumer = _mqService.Session.CreateConsumer(replyQueue, selector))
            {
                ITextMessage message = (ITextMessage)consumer.Receive(timeout);

                if (message == null)
                    throw new TimeoutException(ENoMessageRequestReplyTimeout);

                if (message.NMSCorrelationID != correlationId)
                    throw new InvalidDataException(ECorrelationIdMismatchAborting);

                return message.Text;
            }
        }

        public string CorrelationId => Request.GetCorrelationId().ToString();
    }
}