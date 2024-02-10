using System.Text;
using Domain.Pedidos;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Application.Pedidos.Commands;
using Application.Pedidos.Boundaries;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Application.Pedidos.Queries.DTO;

namespace Infra.RabbitMQ.Consumers
{
    public class PedidoPagoSubscriber : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _nomeDaFila;
        private IConnection _connection;
        private IModel _channel;

        public PedidoPagoSubscriber(
            IServiceScopeFactory scopeFactory,
            RabbitMQOptions options)
        {
            _scopeFactory = scopeFactory;
            _nomeDaFila = options.QueuePedidoPago;

            _connection = new ConnectionFactory()
            {
                HostName = options.Hostname,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
            }.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _channel.QueueDeclare(queue: _nomeDaFila, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _nomeDaFila,
                exchange: "trigger",
                routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var mediatorHandler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var pedidoPago = JsonSerializer.Deserialize<CarrinhoDto>(message);

                    if (pedidoPago is null)
                        return;

                    var input = new AtualizarStatusPedidoInput(pedidoPago.PedidoId, (int)PedidoStatus.Pago);
                    var command = new AtualizarStatusPedidoCommand(input);
                    mediatorHandler.EnviarComando<AtualizarStatusPedidoCommand, bool>(command).Wait();
                }
            };

            _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
