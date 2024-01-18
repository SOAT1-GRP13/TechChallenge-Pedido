using Domain.Base.DomainObjects.DTO;

namespace Domain.Tests.Base.DomainObjects.DTO
{
    public class ListaProdutosPedidoTests
    {
        [Fact]
        public void ListaProdutosPedido_CanBeInitializedAndAccessed()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var quantidade = 5;

            // Act
            var listaProdutosPedido = new ListaProdutosPedido
            {
                PedidoId = pedidoId,
                Itens = new List<Item>
                {
                    new Item { Id = itemId, Quantidade = quantidade }
                }
            };

            // Assert
            Assert.Equal(pedidoId, listaProdutosPedido.PedidoId);
            Assert.Single(listaProdutosPedido.Itens);
            Assert.Equal(itemId, listaProdutosPedido?.Itens.First().Id);
            Assert.Equal(quantidade, listaProdutosPedido?.Itens.First().Quantidade);
        }
    }
}
