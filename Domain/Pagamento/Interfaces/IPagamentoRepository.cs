using Domain.Pedidos;

namespace Domain.Pagamento
{
    public interface IPagamentoRepository
    {
        Task<string> GeraPedidoQrCode(Pedido pedido);
    }
}
