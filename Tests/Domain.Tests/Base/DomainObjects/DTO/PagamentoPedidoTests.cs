using Domain.Base.DomainObjects.DTO;

namespace Domain.Tests.Base.DomainObjects.DTO
{
    public class PagamentoPedidoTests
    {
        [Fact]
        public void PagamentoPedido_CanBeInitialized()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var total = 100.0m;
            var nomeCartao = "Nome Teste";
            var numeroCartao = "1234567890123456";
            var expiracaoCartao = "12/23";
            var cvvCartao = "123";

            // Act
            var pagamentoPedido = new PagamentoPedido
            {
                PedidoId = pedidoId,
                ClienteId = clienteId,
                Total = total,
                NomeCartao = nomeCartao,
                NumeroCartao = numeroCartao,
                ExpiracaoCartao = expiracaoCartao,
                CvvCartao = cvvCartao
            };

            // Assert
            Assert.Equal(pedidoId, pagamentoPedido.PedidoId);
            Assert.Equal(clienteId, pagamentoPedido.ClienteId);
            Assert.Equal(total, pagamentoPedido.Total);
            Assert.Equal(nomeCartao, pagamentoPedido.NomeCartao);
            Assert.Equal(numeroCartao, pagamentoPedido.NumeroCartao);
            Assert.Equal(expiracaoCartao, pagamentoPedido.ExpiracaoCartao);
            Assert.Equal(cvvCartao, pagamentoPedido.CvvCartao);
        }
    }
}
