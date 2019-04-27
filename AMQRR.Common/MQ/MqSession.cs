using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS;

namespace AMQRR.Common.MQ
{
    public class MqSession
    {
        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private ISession _session;

        public MqSession(string brokerUri)
        {
            BrokerUri = brokerUri;
        }

        public string BrokerUri { get; }

        public IConnectionFactory ConnectionFactory => _connectionFactory ?? (_connectionFactory = new NMSConnectionFactory(BrokerUri));

        public IConnection Connection => _connection ?? (_connection = ConnectionFactory.CreateConnection());

        public ISession Session => _session ?? (_session = Connection.CreateSession());
    }
}
