using Domain.Configuration;
using Domain.Pedidos;
using Infra.Pagamento.Repository;
using Microsoft.Extensions.Options;
using System.Net;

namespace Infra.Tests.Pagamento.Repository
{
    public class PagamentoRepositoryTests
    {
        private readonly IOptions<Secrets> _mockOptions;
        private Secrets _settings;

        public PagamentoRepositoryTests()
        {
            _settings = new Secrets { PagamentoApiUrl = "http://fakeapi.com" };
            _mockOptions = Options.Create(_settings);
        }

        [Fact]
        public async Task GeraPedidoQrCode_DeveRetornarStringVazia_QuandoApiRetornaErro()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error message")
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponse);
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri(_settings.PagamentoApiUrl)
            };

            var repository = new PagamentoRepository(_mockOptions);

            var pedido = new Pedido(Guid.NewGuid(), false, 0, 10);

            // Act
            var result = await repository.GeraPedidoQrCode(pedido);

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
