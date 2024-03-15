using FluentValidation;
using Domain.Base.Messages;
using Application.Pedidos.Queries.DTO;

namespace Application.Pedidos.Commands
{
    public class IniciarPedidoCommand : Command<CarrinhoDto>
    {
        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }
        public string ClienteEmail {get; private set;}

        public IniciarPedidoCommand(Guid pedidoId, Guid clienteId, string clienteEmail)
        {
            PedidoId = pedidoId;
            ClienteId = clienteId;
            ClienteEmail = clienteEmail;
        }

        public override bool EhValido()
        {
            ValidationResult = new IniciarPedidoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class IniciarPedidoValidation : AbstractValidator<IniciarPedidoCommand>
    {
        public IniciarPedidoValidation()
        {
            RuleFor(c => c.ClienteId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do cliente inválido");

            RuleFor(c => c.PedidoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do pedido inválido");
        }
    }
}