using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Domain.Base.DomainObjects;
using Microsoft.Extensions.Hosting;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Application.Pedidos.Queries.DTO;

namespace Infra.RabbitMQ.Consumers
{
    public class RabbitMQSubscriber : BackgroundService
    {        
        protected readonly string _nomeDaFila;
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly IModel _channel;

        protected RabbitMQSubscriber(
            string nomeExchage,
            string nomeFila,
            IServiceScopeFactory scopeFactory,
            IModel model)
        {            
            _nomeDaFila = nomeFila;
            _scopeFactory = scopeFactory;
            _channel = model;

            _channel.ExchangeDeclare(exchange: nomeExchage, type: ExchangeType.Fanout);
            _channel.QueueDeclare(queue: _nomeDaFila, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _nomeDaFila,
                exchange: nomeExchage,
                routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) => { InvokeReceivedEvent(ModuleHandle, ea); };

            _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        protected virtual void InvokeReceivedEvent(object? model, BasicDeliverEventArgs ea)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var mediatorHandler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                CarrinhoDto carrinhoDto;
                try
                {
                    carrinhoDto = JsonSerializer.Deserialize<CarrinhoDto>(message) ?? new CarrinhoDto();
                }
                catch (Exception ex)
                {
                    throw new DomainException("Erro deserializar carrinhoDto", ex);
                }

                InvokeCommand(carrinhoDto, mediatorHandler);
            }
        }

        protected virtual void InvokeCommand(CarrinhoDto carrinhoDto, IMediatorHandler mediatorHandler) { }

        public override void Dispose()
        {
            if (_channel.IsOpen)
                _channel.Close();

            GC.SuppressFinalize(this);

            base.Dispose();
        }
    }
}
