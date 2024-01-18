using Application.Pedidos.Queries.DTO;

namespace Application.Tests.Pedidos.Queries.DTO
{
    public class CarrinhoDtoTests
    {
        [Fact]
        public void CarrinhoDto_CanBeInitialized()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var subTotal = 100.0m;
            var valorTotal = 120.0m;

            var item = new CarrinhoItemDto
            {
                ProdutoId = Guid.NewGuid(),
                ProdutoNome = "Produto Teste",
                Quantidade = 2,
                ValorUnitario = 50.0m,
                ValorTotal = 100.0m
            };

            // Act
            var carrinhoDto = new CarrinhoDto
            {
                PedidoId = pedidoId,
                ClienteId = clienteId,
                SubTotal = subTotal,
                ValorTotal = valorTotal,
                Items = new List<CarrinhoItemDto> { item }
            };

            // Assert
            Assert.Equal(pedidoId, carrinhoDto.PedidoId);
            Assert.Equal(clienteId, carrinhoDto.ClienteId);
            Assert.Equal(subTotal, carrinhoDto.SubTotal);
            Assert.Equal(valorTotal, carrinhoDto.ValorTotal);
            Assert.Single(carrinhoDto.Items);
            Assert.Equal(item.ProdutoId, carrinhoDto.Items.First().ProdutoId);
            Assert.Equal(item.ProdutoNome, carrinhoDto.Items.First().ProdutoNome);
            Assert.Equal(item.Quantidade, carrinhoDto.Items.First().Quantidade);
            Assert.Equal(item.ValorUnitario, carrinhoDto.Items.First().ValorUnitario);
            Assert.Equal(item.ValorTotal, carrinhoDto.Items.First().ValorTotal);
        }
    }
}
