using AMQRR.Common.MQ;
using Apache.NMS;

namespace AMQRR.API.Services.Singleton
{
    public interface IMqService
    {
        MqSession MqSession { get; }
        ISession Session { get; }
        IConnection Connection { get; }
    }
}