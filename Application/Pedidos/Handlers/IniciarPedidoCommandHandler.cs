using MediatR;
using Domain.RabbitMQ;
using System.Text.Json;
using Domain.Base.DomainObjects;
using Application.Pedidos.Commands;
using Application.Pedidos.UseCases;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.Configuration;
using Domain.Base.Messages.CommonMessages.Notifications;
using Microsoft.Extensions.Options;
using Domain.Configuration;

namespace Application.Pedidos.Handlers
{
    public class IniciarPedidoCommandHandler : IRequestHandler<IniciarPedidoCommand, CarrinhoDto>
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IPedidoUseCase _pedidoUseCase;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly Secrets _secrets;


        public IniciarPedidoCommandHandler(
            IMediatorHandler mediatorHandler,
            IPedidoUseCase pedidoUseCase,
            IRabbitMQService rabbitMQService,
            IOptions<Secrets> options
            )
        {
            _mediatorHandler = mediatorHandler;
            _pedidoUseCase = pedidoUseCase;
            _rabbitMQService = rabbitMQService;
            _secrets = options.Value;
        }

        public async Task<CarrinhoDto> Handle(IniciarPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido())
            {
                foreach (var error in message.ValidationResult.Errors)
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));

                return new CarrinhoDto();
            }

            try
            {
                var carrinho = await _pedidoUseCase.IniciarPedido(message.PedidoId);
                carrinho.ClienteEmail = message.ClienteEmail;

                string mensagem = JsonSerializer.Serialize(carrinho);
                var fila = _secrets.ExchangePedidoConfirmado;
                _rabbitMQService.PublicaMensagem(fila, mensagem);

                return carrinho;
            }
            catch (DomainException ex)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(message.MessageType, ex.Message));
                return new CarrinhoDto();
            }
        }
    }
}
