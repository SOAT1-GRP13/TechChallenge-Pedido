using Domain.Catalogo;
using Domain.Configuration;
using Infra.Catalogo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Tests.Catalogo.Repository
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    public class ProdutoRepositoryTests
    {
        private readonly IConfiguration _mockConfiguration;
        private readonly ILogger<ProdutoRepository> _mockLogger;
        private readonly IOptions<Secrets> _mockOptions;
        private Secrets _secrets;

        public ProdutoRepositoryTests()
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
            var produto = new Produto("Produto 1", "Descrição do produto 1", true, 10, Guid.NewGuid(), DateTime.Now, "imagem.png");
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
