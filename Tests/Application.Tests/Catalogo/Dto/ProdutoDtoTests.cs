using Application.Catalogo.Dto;

namespace Application.Tests.Catalogo.Dto
{
    public class ProdutoDtoTests
    {
        [Fact]
        public void ProdutoDto_SetAndGetValues_ReturnsCorrectValues()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var nome = "Teste Produto";
            var descricao = "Descrição do Teste";
            var ativo = true;
            var valor = 100.0m;
            var dataCadastro = DateTime.UtcNow;
            var imagem = "data:image/png;base64,iVBORw0KGgo=";

            // Act
            var produtoDto = new ProdutoDto
            {
                Id = produtoId,
                CategoriaId = categoriaId,
                Nome = nome,
                Descricao = descricao,
                Ativo = ativo,
                Valor = valor,
                DataCadastro = dataCadastro,
                Imagem = imagem
            };

            // Assert
            Assert.Equal(produtoId, produtoDto.Id);
            Assert.Equal(categoriaId, produtoDto.CategoriaId);
            Assert.Equal(nome, produtoDto.Nome);
            Assert.Equal(descricao, produtoDto.Descricao);
            Assert.Equal(ativo, produtoDto.Ativo);
            Assert.Equal(valor, produtoDto.Valor);
            Assert.Equal(dataCadastro, produtoDto.DataCadastro);
            Assert.Equal(imagem, produtoDto.Imagem);
        }
    }
}
