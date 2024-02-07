using System.Text;
using Domain.RabbitMQ;
using RabbitMQ.Client;

namespace Infra.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQOptions _options;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQService(RabbitMQOptions options)
        {
            _options = options;
            var factory = new ConnectionFactory()
            {
                HostName = _options.Hostname,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void PublicaMensagem(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);
        }
    }

}
