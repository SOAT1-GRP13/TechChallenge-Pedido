using Application.Pedidos.Queries.DTO;

namespace Application.Tests.Pedidos.Queries.DTO
{
    public class CarrinhoItemDtoTests
    {
        [Fact]
        public void CarrinhoItemDto_AssignsAndRetrievesValuesCorrectly()
        {
            // Arrange
            var carrinhoItemDto = new CarrinhoItemDto();
            var expectedProdutoId = Guid.NewGuid();
            var expectedProdutoNome = "Produto Teste";
            var expectedQuantidade = 5;
            var expectedValorUnitario = 10.00m;
            var expectedValorTotal = 50.00m;

            // Act
            carrinhoItemDto.ProdutoId = expectedProdutoId;
            carrinhoItemDto.ProdutoNome = expectedProdutoNome;
            carrinhoItemDto.Quantidade = expectedQuantidade;
            carrinhoItemDto.ValorUnitario = expectedValorUnitario;
            carrinhoItemDto.ValorTotal = expectedValorTotal;

            // Assert
            Assert.Equal(expectedProdutoId, carrinhoItemDto.ProdutoId);
            Assert.Equal(expectedProdutoNome, carrinhoItemDto.ProdutoNome);
            Assert.Equal(expectedQuantidade, carrinhoItemDto.Quantidade);
            Assert.Equal(expectedValorUnitario, carrinhoItemDto.ValorUnitario);
            Assert.Equal(expectedValorTotal, carrinhoItemDto.ValorTotal);
        }
    }
}
