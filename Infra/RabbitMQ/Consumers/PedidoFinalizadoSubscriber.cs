using Domain.Pedidos;
using RabbitMQ.Client;
using Application.Pedidos.Commands;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.RabbitMQ.Consumers
{
    public class PedidoFinalizadoSubscriber : RabbitMQSubscriber
    {
        public PedidoFinalizadoSubscriber(IServiceScopeFactory scopeFactory, RabbitMQOptions options, IModel model) : base(scopeFactory, options.QueuePedidoFinalizado, model) { }

        protected override void InvokeCommand(CarrinhoDto carrinhoDto, IMediatorHandler mediatorHandler)
        {
            var input = new AtualizarStatusPedidoInput(carrinhoDto.PedidoId, (int)PedidoStatus.Finalizado);
            var command = new AtualizarStatusPedidoCommand(input);
            mediatorHandler.EnviarComando<AtualizarStatusPedidoCommand, bool>(command).Wait();
        }
    }
}
