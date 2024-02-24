using Domain.Pedidos;
using RabbitMQ.Client;
using Application.Pedidos.Commands;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;


namespace Infra.RabbitMQ.Consumers
{
    public class PedidoPreparandoSubscriber : RabbitMQSubscriber
    {
        public PedidoPreparandoSubscriber(IServiceScopeFactory scopeFactory, RabbitMQOptions options, IModel model) : base(options.ExchangePedidoPreparando, options.QueuePedidoPreparando, scopeFactory, model) { }

        protected override void InvokeCommand(CarrinhoDto carrinhoDto, IMediatorHandler mediatorHandler)
        {
            var input = new AtualizarStatusPedidoInput(carrinhoDto.PedidoId, (int)PedidoStatus.EmPreparacao);
            var command = new AtualizarStatusPedidoCommand(input);
            mediatorHandler.EnviarComando<AtualizarStatusPedidoCommand, bool>(command).Wait();
        }
    }
}
