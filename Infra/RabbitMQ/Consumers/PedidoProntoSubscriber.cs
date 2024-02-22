using Domain.Pedidos;
using RabbitMQ.Client;
using Application.Pedidos.Commands;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;


namespace Infra.RabbitMQ.Consumers
{
    public class PedidoProntoSubscriber : RabbitMQSubscriber
    {
        public PedidoProntoSubscriber(IServiceScopeFactory scopeFactory, RabbitMQOptions options, IModel model) : base(scopeFactory, options.QueuePedidoPronto, model) { }

        protected override void InvokeCommand(CarrinhoDto carrinhoDto, IMediatorHandler mediatorHandler)
        {
            var input = new AtualizarStatusPedidoInput(carrinhoDto.PedidoId, (int)PedidoStatus.Pronto);
            var command = new AtualizarStatusPedidoCommand(input);
            mediatorHandler.EnviarComando<AtualizarStatusPedidoCommand, bool>(command).Wait();
        }
    }
}
