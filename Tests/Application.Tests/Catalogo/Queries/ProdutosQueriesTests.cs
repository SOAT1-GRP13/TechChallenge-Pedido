using Application.Catalogo.Dto;
using Application.Catalogo.Queries;
using AutoMapper;
using Domain.Catalogo;
using Moq;

namespace Application.Tests.Catalogo.Queries
{
    public class ProdutosQueriesTests
    {
        [Fact]
        public async Task ObterPorId_DeveRetornarProdutoDto_QuandoProdutoExiste()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Teste", "Descricao Teste", true, 100, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");

            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            produtoRepositoryMock.Setup(repo => repo.ObterPorId(produtoId)).ReturnsAsync(produto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<ProdutoDto>(It.IsAny<Produto>())).Returns(new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Valor = produto.Valor,
                Ativo = produto.Ativo,
                CategoriaId = produto.CategoriaId,
                DataCadastro = produto.DataCadastro,
                Imagem = produto.Imagem
            });

            var produtosQueries = new ProdutosQueries(produtoRepositoryMock.Object, mapperMock.Object);

            // Act
            var produtoDto = await produtosQueries.ObterPorId(produtoId);

            // Assert
            Assert.NotNull(produtoDto);
            Assert.Equal(produto.Nome, produtoDto.Nome);
            Assert.Equal(produto.Descricao, produtoDto.Descricao);
            Assert.Equal(produto.Valor, produtoDto.Valor);
            Assert.Equal(produto.Ativo, produtoDto.Ativo);
            Assert.Equal(produto.CategoriaId, produtoDto.CategoriaId);
            Assert.Equal(produto.DataCadastro, produtoDto.DataCadastro);
            Assert.Equal(produto.Imagem, produtoDto.Imagem);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNull_QuandoProdutoNaoExiste()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            produtoRepositoryMock.Setup(repo => repo.ObterPorId(produtoId)).ReturnsAsync((Produto)null);

            var mapperMock = new Mock<IMapper>();

            var produtosQueries = new ProdutosQueries(produtoRepositoryMock.Object, mapperMock.Object);

            // Act
            var produtoDto = await produtosQueries.ObterPorId(produtoId);

            // Assert
            Assert.Null(produtoDto);
        }
    }
}
