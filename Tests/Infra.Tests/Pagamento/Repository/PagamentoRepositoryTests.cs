using Domain.Catalogo;
using Domain.Configuration;
using Infra.Catalogo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace Infra.Tests.Pagamento.Repository
{
    public class PagamentoRepositoryTests
    {
        private readonly IConfiguration _mockConfiguration;
        private readonly ILogger<ProdutoRepository> _mockLogger;
        private readonly IOptions<Secrets> _mockOptions;
        private Secrets _secrets;

        public PagamentoRepositoryTests()
        {
            _mockConfiguration = Mock.Of<IConfiguration>();
            _mockLogger = Mock.Of<ILogger<ProdutoRepository>>();
            _secrets = new Secrets { CatalogoApiUrl = "http://fakeapi.com" };
            _mockOptions = Options.Create(_secrets);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarProduto_QuandoApiRetornaSucesso()
        {
            // Arrange
            var produto = new Produto("Teste", "Teste", true, 10, Guid.NewGuid(), DateTime.Now, "Teste");
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(produto))
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponse);
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            var repository = new ProdutoRepository(_mockConfiguration, fakeHttpClient, _mockLogger, _mockOptions);

            // Act
            var result = await repository.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNull_QuandoApiRetornaErro()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error message")
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponse);
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            var repository = new ProdutoRepository(_mockConfiguration, fakeHttpClient, _mockLogger, _mockOptions);

            // Act
            var result = await repository.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
