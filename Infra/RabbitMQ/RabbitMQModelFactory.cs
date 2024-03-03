using Domain.Configuration;
using Domain.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Infra.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQModelFactory
    {
        private readonly Secrets _settings;

        public RabbitMQModelFactory(IOptions<Secrets> options)
        {
            _settings = options.Value;
        }

        public IModel CreateModel()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Rabbit_Hostname,
                Port = Convert.ToInt32(_settings.Rabbit_Port),
                UserName = _settings.Rabbit_Username,
                Password = _settings.Rabbit_Password,
                VirtualHost = _settings.Rabbit_VirtualHost
            };

            var connection = factory.CreateConnection();

            return connection.CreateModel();
        }
    }
}
