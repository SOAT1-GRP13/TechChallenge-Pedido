using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;


namespace Application.Pedidos.Queries
{
    public interface IPedidoQueries
    {
        Task<CarrinhoDto> ObterCarrinhoCliente(Guid clienteId);
    }
}
