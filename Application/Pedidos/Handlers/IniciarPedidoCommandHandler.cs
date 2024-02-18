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

namespace Application.Pedidos.Handlers
{
    public class IniciarPedidoCommandHandler : IRequestHandler<IniciarPedidoCommand, CarrinhoDto>
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IPedidoUseCase _pedidoUseCase;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConfiguration _configuration;


        public IniciarPedidoCommandHandler(
            IMediatorHandler mediatorHandler,
            IPedidoUseCase pedidoUseCase,
            IRabbitMQService rabbitMQService,
            IConfiguration configuration)
        {
            _mediatorHandler = mediatorHandler;
            _pedidoUseCase = pedidoUseCase;
            _rabbitMQService = rabbitMQService;
            _configuration = configuration;
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

                string mensagem = JsonSerializer.Serialize(carrinho);
                var fila = _configuration.GetSection("RabbitMQ:QueuePedidoConfirmado").Value;
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
