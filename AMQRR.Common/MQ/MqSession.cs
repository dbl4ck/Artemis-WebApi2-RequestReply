using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;

namespace AMQRR.Common.MQ
{
    public class MqSession
    {
        private string _brokerUri;
        private bool _autoStart;
        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private ISession _session;

        public MqSession(string brokerUri, bool autoStart = true)
        {
            _brokerUri = brokerUri;
            _autoStart = autoStart;
        }

        public string BrokerUri => _brokerUri;

        public IConnectionFactory ConnectionFactory => _connectionFactory ?? (_connectionFactory = new ConnectionFactory(BrokerUri));

        //public IConnection Connection => _connection ?? (_connection = ConnectionFactory.CreateConnection());

        public IConnection Connection
        {
            get
            {
                if (_connection == null) _connection = ConnectionFactory.CreateConnection(); ;

                if (_autoStart && !_connection.IsStarted) _connection.Start();
                
                return _connection;
            }
        }
        
        public ISession Session => _session ?? (_session = Connection.CreateSession());
    }
}
