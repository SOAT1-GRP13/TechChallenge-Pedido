using Domain.Base.DomainObjects;
using Domain.Catalogo;

namespace Domain.Tests.Catalogo
{
    public class CategoriaTests
    {
        [Fact]
        public void Categoria_CriacaoComParametrosValidos_DeveCriarCategoria()
        {
            // Arrange
            var nome = "Eletrônicos";
            var codigo = 1;

            // Act
            var categoria = new Categoria(nome, codigo);

            // Assert
            Assert.Equal(nome, categoria.Nome);
            Assert.Equal(codigo, categoria.Codigo);
        }

        [Fact]
        public void Categoria_CriacaoComNomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var nome = string.Empty;
            var codigo = 1;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Categoria(nome, codigo));
            Assert.Equal("O campo Nome da categoria não pode estar vazio", exception.Message);
        }

        [Fact]
        public void Categoria_CriacaoComCodigoZero_DeveLancarExcecao()
        {
            // Arrange
            var nome = "Eletrônicos";
            var codigo = 0;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Categoria(nome, codigo));
            Assert.Equal("O campo Codigo não pode ser 0", exception.Message);
        }
    }
}
