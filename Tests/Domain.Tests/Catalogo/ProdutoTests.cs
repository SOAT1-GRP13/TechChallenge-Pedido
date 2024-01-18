using Domain.Base.DomainObjects;
using Domain.Catalogo;

namespace Domain.Tests.Catalogo
{
    public class ProdutoTests
    {
        [Fact]
        public void AoAtivar_DeveAlterarEstadoParaAtivo()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", false, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");

            // Act
            produto.Ativar();

            // Assert
            Assert.True(produto.Ativo);
        }

        [Fact]
        public void AoDesativar_DeveAlterarEstadoParaInativo()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");

            // Act
            produto.Desativar();

            // Assert
            Assert.False(produto.Ativo);
        }

        [Fact]
        public void AoAlterarCategoria_DeveAtualizarCategoriaDoProduto()
        {
            // Arrange
            var categoriaInicial = new Categoria("Categoria Inicial", 1);
            var categoriaNova = new Categoria("Categoria Nova", 2);
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, categoriaInicial.Id, DateTime.UtcNow, "imagem.jpg");

            // Associar a categoria inicial ao produto
            produto.AlterarCategoria(categoriaInicial);

            // Act
            produto.AlterarCategoria(categoriaNova);

            // Assert
            Assert.Equal(categoriaNova.Id, produto.CategoriaId);
            Assert.Equal(categoriaNova, produto.Categoria);
        }

        [Fact]
        public void AoAlterarDescricao_DeveAtualizarDescricaoDoProduto()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição Antiga", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            var novaDescricao = "Descrição Nova";

            // Act
            produto.AlterarDescricao(novaDescricao);

            // Assert
            Assert.Equal(novaDescricao, produto.Descricao);
        }

        [Fact]
        public void AoAlterarDescricao_DeveFalharParaDescricaoVazia()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição Antiga", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            var descricaoVazia = "";

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => produto.AlterarDescricao(descricaoVazia));
            Assert.Equal("O campo Descricao do produto não pode estar vazio", exception.Message);
        }

        [Fact]
        public void AoDebitarEstoque_DeveDiminuirQuantidadeEstoque()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(10); // Adiciona 10 itens ao estoque

            // Act
            produto.DebitarEstoque(3); // Debita 3 itens do estoque

            // Assert
            Assert.Equal(7, produto.QuantidadeEstoque); // Espera-se que o estoque seja 7
        }

        [Fact]
        public void AoDebitarEstoque_DeveFalharQuandoEstoqueInsuficiente()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(5); // Adiciona 5 itens ao estoque

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => produto.DebitarEstoque(10)); // Tentativa de debitar 10 itens
            Assert.Equal("Estoque insuficiente", exception.Message);
        }

        [Fact]
        public void AoReporEstoque_DeveAumentarQuantidadeEstoque()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(5);
            
            // Act
            produto.ReporEstoque(10);

            // Assert
            Assert.Equal(15, produto.QuantidadeEstoque);
        }

        [Fact]
        public void AoReporEstoque_DevePermitirQuantidadeZero()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");

            // Act
            produto.ReporEstoque(0);

            // Assert
            Assert.Equal(0, produto.QuantidadeEstoque);
        }

        [Fact]
        public void AoPossuiEstoque_DeveRetornarVerdadeiro_QuandoEstoqueSuficiente()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(10);

            // Act
            var resultado = produto.PossuiEstoque(5);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void AoPossuiEstoque_DeveRetornarFalso_QuandoEstoqueInsuficiente()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(3);

            // Act
            var resultado = produto.PossuiEstoque(5);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void AoPossuiEstoque_DeveRetornarVerdadeiro_QuandoEstoqueExato()
        {
            // Arrange
            var produto = new Produto("Teste Produto", "Descrição do produto", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg");
            produto.ReporEstoque(5);

            // Act
            var resultado = produto.PossuiEstoque(5);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void Validar_DeveLancarExcecao_QuandoNomeVazio()
        {
            // Arrange & Act & Assert
            var excecao = Assert.Throws<DomainException>(() =>
                new Produto("", "Descrição", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg"));

            Assert.Equal("O campo Nome do produto não pode estar vazio", excecao.Message);
        }

        [Fact]
        public void Validar_DeveLancarExcecao_QuandoDescricaoVazio()
        {
            // Arrange & Act & Assert
            var excecao = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg"));

            Assert.Equal("O campo Descricao do produto não pode estar vazio", excecao.Message);
        }

        [Fact]
        public void Validar_DeveLancarExcecao_QuandoImagemVazio()
        {
            // Arrange & Act & Assert
            var excecao = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "Descricao", true, 100m, Guid.NewGuid(), DateTime.UtcNow, ""));

            Assert.Equal("O campo Imagem do produto não pode estar vazio", excecao.Message);
        }

        [Fact]
        public void Validar_DeveLancarExcecao_QuandoCategoriaIdVazio()
        {
            // Arrange & Act & Assert
            var excecao = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "Descricao", true, 100m, Guid.Empty, DateTime.UtcNow, "imagem.jpg"));

            Assert.Equal("O campo CategoriaId do produto não pode estar vazio", excecao.Message);
        }

        [Fact]
        public void Validar_DeveLancarExcecao_QuandoValorMenorOuIgualZero()
        {
            // Arrange & Act & Assert
            var excecao = Assert.Throws<DomainException>(() =>
                new Produto("Produto", "Descrição", true, 0, Guid.NewGuid(), DateTime.UtcNow, "imagem.jpg"));

            Assert.Equal("O campo Valor do produto não pode se menor igual a 0", excecao.Message);
        }
    }
}
