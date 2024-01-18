using Domain.Pagamento;
using Domain.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Pagamento
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_CanBeInitialized()
        {
            // Arrange
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 50);

            // Act
            var orderItem = new OrderItem(pedidoItem);

            // Assert
            Assert.Equal("Produto Teste", orderItem.Title);
            Assert.Equal("Observação do item", orderItem.Description);
            Assert.Equal(50, orderItem.Unit_price);
            Assert.Equal(2, orderItem.Quantity);
            Assert.Equal("unit", orderItem.Unit_measure);
            Assert.Equal(100, orderItem.Total_amount);
        }

        [Fact]
        public void OrderItem_ConstrutorVazio_CanBeInitialized()
        {

            // Act
            var orderItem = new OrderItem();

            // Assert
            Assert.Equal(0, orderItem.Unit_price);
        }
    }
}
