using Domain.Base.DomainObjects;

namespace Domain.Pedidos
{
    public class Pedido : Entity, IAggregateRoot
    {
        public int Codigo { get; private set; }
        public Guid ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }

        private readonly List<PedidoItem> _pedidoItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;

        public Pedido(Guid clienteId, decimal valorTotal)
        {
            ClienteId = clienteId;
            ValorTotal = valorTotal;
            _pedidoItems = new List<PedidoItem>();
        }

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }


        public void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(p => p.CalcularValor());
        }


        public bool PedidoItemExistente(PedidoItem item)
        {
            return _pedidoItems.Any(p => p.ProdutoId == item.ProdutoId);
        }

        public void AdicionarItem(PedidoItem item)
        {
            if (!item.EhValido()) return;

            item.AssociarPedido(Id);

            if (PedidoItemExistente(item))
            {
                var itemExistente = _pedidoItems.First(p => p.ProdutoId == item.ProdutoId);
                itemExistente.AdicionarUnidades(item.Quantidade);
                item = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            item.CalcularValor();
            _pedidoItems.Add(item);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem item)
        {
            if (!item.EhValido()) return;

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

            if (itemExistente is null) throw new DomainException("O item não pertence ao pedido");

            _pedidoItems.Remove(itemExistente);

            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem item)
        {
            if (!item.EhValido()) return;
            item.AssociarPedido(Id);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

            if (itemExistente is null) 
                throw new DomainException("O item não pertence ao pedido");

            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(item);

            CalcularValorPedido();
        }

        public void AtualizarUnidades(PedidoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        public void TornarRascunho()
        {
            if (PedidoStatus != PedidoStatus.Rascunho)
                throw new DomainException("O pedido já está em um status diferente de rascunho");

            PedidoStatus = PedidoStatus.Rascunho;
        }

        public void IniciarPedido()
        {
            if (PedidoStatus != PedidoStatus.Rascunho)
                throw new DomainException("O pedido não pode ser iniciado, pois não está em rascunho");

            PedidoStatus = PedidoStatus.Iniciado;
        }

        public void RecusarPedido()
        {
            if (PedidoStatus != PedidoStatus.Iniciado)
                throw new DomainException("Pedido não pode ser recusado, pois o mesmo não está iniciado");

            PedidoStatus = PedidoStatus.Recusado;
        }

        public void ColocarPedidoComoPago()
        {
            if (PedidoStatus != PedidoStatus.Iniciado)
                throw new DomainException("Pedido não pode ser colocado como pago, pois o mesmo não está iniciado");

            PedidoStatus = PedidoStatus.Pago;
        }

        public void CancelarPedido()
        {
            if (PedidoStatus == PedidoStatus.Cancelado)
                throw new DomainException("Pedido já está cancelado");

            if (PedidoStatus == PedidoStatus.EmPreparacao)
                throw new DomainException("Pedido não pode ser cancelado, pois já foi para preparação");

            if (PedidoStatus == PedidoStatus.Pronto)
                throw new DomainException("Pedido não pode ser cancelado, pois já está pronto");

            if (PedidoStatus == PedidoStatus.Finalizado)
                throw new DomainException("Pedido já foi finalizado");

            if (PedidoStatus == PedidoStatus.Recusado)
                throw new DomainException("Pedido já foi recusado");

            PedidoStatus = PedidoStatus.Cancelado;
        }

        public void ColocarPedidoComoPronto()
        {
            if (PedidoStatus != PedidoStatus.EmPreparacao)
                throw new DomainException("Pedido não pode ser colocado como pronto, pois o mesmo não está em preparação");

            PedidoStatus = PedidoStatus.Pronto;
        }

        public void ColocarPedidoEmPreparacao()
        {
            if (PedidoStatus != PedidoStatus.Recebido)
                throw new DomainException("Pedido não pode ser colocado em preparação, pois o mesmo não foi recebido");

            PedidoStatus = PedidoStatus.EmPreparacao;
        }

        public void ColocarPedidoComoRecebido()
        {
            if (PedidoStatus != PedidoStatus.Pago)
                throw new DomainException("Pedido não pode ser colocado como recebido, pois o mesmo não foi pago");

            PedidoStatus = PedidoStatus.Recebido;
        }

        public void FinalizarPedido()
        {
            if (PedidoStatus != PedidoStatus.Pronto)
                throw new DomainException("Pedido não pode ser finalizado, pois não está pronto");

            PedidoStatus = PedidoStatus.Finalizado;
        }

        public void AtualizarStatus(PedidoStatus status)
        {
            switch (status)
            {
                case PedidoStatus.Rascunho:
                    TornarRascunho();
                    break;
                case PedidoStatus.Iniciado:
                    IniciarPedido();
                    break;
                case PedidoStatus.Pago:
                    ColocarPedidoComoPago();
                    break;
                case PedidoStatus.Cancelado:
                    CancelarPedido();
                    break;
                case PedidoStatus.Pronto:
                    ColocarPedidoComoPronto();
                    break;
                case PedidoStatus.EmPreparacao:
                    ColocarPedidoEmPreparacao();
                    break;
                case PedidoStatus.Recebido:
                    ColocarPedidoComoRecebido();
                    break;
                case PedidoStatus.Finalizado:
                    FinalizarPedido();
                    break;
                case PedidoStatus.Recusado:
                    RecusarPedido();
                    break;
                default:
                    throw new DomainException("Status do pedido inválido");
            }
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId,
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}
