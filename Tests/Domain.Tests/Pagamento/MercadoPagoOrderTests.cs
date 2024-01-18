using Domain.Pagamento;
using Domain.Pedidos;

namespace Domain.Tests.Pagamento
{
    public class MercadoPagoOrderTests
    {
        [Fact]
        public void MercadoPagoOrder_CanBeInitialized()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), false, 0, 100);
            var orderItems = new List<OrderItem> { new OrderItem(new PedidoItem(Guid.NewGuid(), "Produto Teste", 1, 100)) };

            // Act
            var mercadoPagoOrder = new MercadoPagoOrder(pedido, orderItems);

            // Assert
            Assert.Equal(pedido.Id.ToString(), mercadoPagoOrder.External_reference);
            Assert.Equal("Pedido confirmado", mercadoPagoOrder.Title);
            Assert.Equal("Descrição do pedido", mercadoPagoOrder.Description);
            Assert.Equal(100, mercadoPagoOrder.Total_amount);
            Assert.NotEmpty(mercadoPagoOrder.Items);
        }

        [Fact]
        public void MercadoPagoOrder_ConstrutorVazio_CanBeInitialized()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), false, 0, 100);

            // Act
            var mercadoPagoOrder = new MercadoPagoOrder();

            // Assert
            Assert.Equal(0, mercadoPagoOrder.Total_amount);
        }
    }
}
