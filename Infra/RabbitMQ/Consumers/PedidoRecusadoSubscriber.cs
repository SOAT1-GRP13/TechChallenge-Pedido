using Domain.Pedidos;
using RabbitMQ.Client;
using Application.Pedidos.Commands;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.RabbitMQ.Consumers
{
    public class PedidoRecusadoSubscriber : RabbitMQSubscriber
    {
        public PedidoRecusadoSubscriber(IServiceScopeFactory scopeFactory, RabbitMQOptions options, IModel model) : base(options.ExchangePedidoRecusado, options.QueuePedidoRecusado, scopeFactory, model) { }

        protected override void InvokeCommand(CarrinhoDto carrinhoDto, IMediatorHandler mediatorHandler)
        {
            var input = new AtualizarStatusPedidoInput(carrinhoDto.PedidoId, (int)PedidoStatus.Recusado);
            var command = new AtualizarStatusPedidoCommand(input);
            mediatorHandler.EnviarComando<AtualizarStatusPedidoCommand, bool>(command).Wait();
        }
    }
}
