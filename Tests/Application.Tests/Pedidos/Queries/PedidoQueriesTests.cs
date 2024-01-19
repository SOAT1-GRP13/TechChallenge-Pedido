using Application.Pedidos.Queries;
using Domain.Pedidos;
using Moq;

namespace Application.Tests.Pedidos.Queries
{
    public class PedidoQueriesTests
    {
        [Fact]
        public async Task ObterCarrinhoCliente_DeveRetornarCarrinho_QuandoPedidoExiste()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 50);
            var pedido = new Pedido(clienteId, false, 0, 100);
            pedido.TornarRascunho();
            pedido.AdicionarItem(pedidoItem);

            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            pedidoRepositoryMock.Setup(repo => repo.ObterPedidoRascunhoPorClienteId(clienteId))
                                .ReturnsAsync(pedido);

            var pedidoQueries = new PedidoQueries(pedidoRepositoryMock.Object);

            // Act
            var carrinhoDto = await pedidoQueries.ObterCarrinhoCliente(clienteId);

            // Assert
            Assert.NotNull(carrinhoDto);
            Assert.Equal(clienteId, carrinhoDto.ClienteId);
            Assert.Equal(pedido.ValorTotal, carrinhoDto.ValorTotal);
            Assert.Single(carrinhoDto.Items);
            Assert.Equal(produtoId, carrinhoDto.Items[0].ProdutoId);
            Assert.Equal(2, carrinhoDto.Items[0].Quantidade);
            Assert.Equal(50, carrinhoDto.Items[0].ValorUnitario);
            Assert.Equal(100, carrinhoDto.Items[0].ValorTotal);
        }

        [Fact]
        public async Task ObterCarrinhoCliente_DeveRetornarNull_QuandoPedidoNaoExiste()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            pedidoRepositoryMock.Setup(repo => repo.ObterPedidoRascunhoPorClienteId(clienteId))
                                .ReturnsAsync((Pedido)null);

            var pedidoQueries = new PedidoQueries(pedidoRepositoryMock.Object);

            // Act
            var carrinhoDto = await pedidoQueries.ObterCarrinhoCliente(clienteId);

            // Assert
            Assert.Null(carrinhoDto);
        }
    }
}
