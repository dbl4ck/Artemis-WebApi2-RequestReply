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

        private const string ECorrelationIdMismatchAborting = "CorrelationId mismatch.";
        private const string ENullMessage = "Received a null message from the consumer. This could be due to a timeout waiting for a relevant message in a reply queue.";

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

        protected string ExecuteRequestReply(string data, string queueName, string replyQueueName, string correlationId, int timeoutSecs = HTTP_TIMEOUT_SECONDS)
        {
            var timeout = TimeSpan.FromSeconds(timeoutSecs);
            var requestQueue = SessionUtil.GetDestination(Session, queueName);
            var replyQueue = SessionUtil.GetDestination(Session, replyQueueName);

            Produce(Session, data, requestQueue, timeout, replyQueue, correlationId);

            return Consume(Session, replyQueue, correlationId, timeout);
        }

        protected void ExecutePointToPoint(string data, string queueName, int? expirySecs = null)
        {
            var expiry = expirySecs.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(expirySecs.Value) : null;
            var queue = SessionUtil.GetDestination(Session, queueName);
            
            Produce(Session, data, queue, expiry);
        }

        private void Produce(ISession session, string data, IDestination queue, TimeSpan? expiry = null, IDestination replyTo = null, string correlationId = null)
        {
            using (var producer = _mqService.MqSession.Session.CreateProducer(queue))
            {
                var message = _mqService.Session.CreateTextMessage(data);

                if(expiry.HasValue)
                    message.NMSTimeToLive = expiry.Value;

                if(replyTo != null)
                    message.NMSReplyTo = replyTo;

                if(correlationId != null)
                    message.NMSCorrelationID = correlationId;

                producer.Send(message);
            }
        }

        private string Consume(ISession session, IDestination queue, string correlationId, TimeSpan timeout)
        {
            var selector = new MqFilterGenerator().Add(NMSFilter.JMSCorrelationID, correlationId).ToString();

            using (var consumer = _mqService.Session.CreateConsumer(queue, selector))
            {
                var message = (ITextMessage)consumer.Receive(timeout);

                if (message == null)
                    throw new TimeoutException(ENullMessage);

                if (message.NMSCorrelationID != correlationId)
                    throw new InvalidDataException(ECorrelationIdMismatchAborting);

                return message.Text;
            }
        }

        protected string CorrelationId => Request.GetCorrelationId().ToString();
        protected ISession Session => _mqService.Session;
        protected IConnection Connection => _mqService.Connection;
    }
}