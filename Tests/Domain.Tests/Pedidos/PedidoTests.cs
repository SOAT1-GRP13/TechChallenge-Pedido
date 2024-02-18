using Domain.Base.DomainObjects;
using Domain.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Pedidos
{
    public class PedidoTests
    {
        #region CalcularValorPedido
        [Fact]
        public void CalcularValorPedido_DeveCalcularValorCorreto_QuandoPedidoTemItens()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 0); // Inicializa o pedido com valor total 0

            var produtoId1 = Guid.NewGuid();
            var produtoNome1 = "Produto 1";
            var quantidade1 = 2;
            var valorUnitario1 = 50m; // Valor total esperado: 100

            var produtoId2 = Guid.NewGuid();
            var produtoNome2 = "Produto 2";
            var quantidade2 = 1;
            var valorUnitario2 = 30m; // Valor total esperado: 30

            var item1 = new PedidoItem(produtoId1, produtoNome1, quantidade1, valorUnitario1);
            var item2 = new PedidoItem(produtoId2, produtoNome2, quantidade2, valorUnitario2);

            pedido.AdicionarItem(item1);
            pedido.AdicionarItem(item2);

            var valorTotalEsperado = (quantidade1 * valorUnitario1) + (quantidade2 * valorUnitario2);

            // Act
            pedido.CalcularValorPedido();

            // Assert
            Assert.Equal(valorTotalEsperado, pedido.ValorTotal);
        }
        #endregion

        #region PedidoItemExistente
        [Fact]
        public void PedidoItemExistente_DeveRetornarTrue_QuandoItemExiste()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 0);

            var produtoId = Guid.NewGuid();
            var item = new PedidoItem(produtoId, "Produto Teste", 1, 100m);

            // Adicionando o item ao pedido
            pedido.AdicionarItem(item);

            // Act
            var resultado = pedido.PedidoItemExistente(new PedidoItem(produtoId, "Produto Teste", 1, 100m));

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void PedidoItemExistente_DeveRetornarFalse_QuandoItemNaoExiste()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 0);

            var produtoIdExistente = Guid.NewGuid();
            var itemExistente = new PedidoItem(produtoIdExistente, "Produto Teste", 1, 100m);

            // Adicionando um item diferente ao pedido
            pedido.AdicionarItem(itemExistente);

            var produtoIdNovo = Guid.NewGuid();

            // Act
            var resultado = pedido.PedidoItemExistente(new PedidoItem(produtoIdNovo, "Produto Novo", 1, 100m));

            // Assert
            Assert.False(resultado);
        }
        #endregion

        #region AdicionarItem
        [Fact]
        public void AdicionarItem_DeveAdicionarNovoItem_QuandoItemNaoExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            var item = new PedidoItem(produtoId, "Produto Teste", 1, 100m);

            // Act
            pedido.AdicionarItem(item);

            // Assert
            Assert.Single(pedido.PedidoItems);
            Assert.Contains(pedido.PedidoItems, i => i.ProdutoId == produtoId && i.Quantidade == 1);
        }

        [Fact]
        public void AdicionarItem_DeveAtualizarQuantidade_QuandoItemJaExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            var itemOriginal = new PedidoItem(produtoId, "Produto Teste", 1, 100m);
            pedido.AdicionarItem(itemOriginal);

            var itemAdicional = new PedidoItem(produtoId, "Produto Teste", 2, 100m); // Mesmo produtoId, quantidade diferente

            // Act
            pedido.AdicionarItem(itemAdicional);

            // Assert
            Assert.Single(pedido.PedidoItems); // Ainda deve haver apenas um item, mas com quantidade atualizada
            Assert.Equal(3, pedido.PedidoItems.First(i => i.ProdutoId == produtoId).Quantidade); // Quantidade deve ser a soma das duas adições
        }
        #endregion

        #region RemoverItem
        [Fact]
        public void RemoverItem_DeveRemoverItem_QuandoItemExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            var item = new PedidoItem(produtoId, "Produto Teste", 1, 100m);
            pedido.AdicionarItem(item);

            // Act
            pedido.RemoverItem(item);

            // Assert
            Assert.DoesNotContain(pedido.PedidoItems, i => i.ProdutoId == produtoId);
        }

        [Fact]
        public void RemoverItem_DeveLancarExcecao_QuandoItemNaoExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var itemInexistente = new PedidoItem(Guid.NewGuid(), "Produto Inexistente", 1, 100m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => pedido.RemoverItem(itemInexistente));
            Assert.Equal("O item não pertence ao pedido", exception.Message);
        }
        #endregion

        #region AtualizarItem
        [Fact]
        public void AtualizarItem_DeveAtualizarItem_QuandoItemExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var itemOriginal = new PedidoItem(produtoId, "Produto Teste", 1, 100m);
            pedido.AdicionarItem(itemOriginal);

            var itemAtualizado = new PedidoItem(produtoId, "Produto Teste Atualizado", 2, 200m);

            // Act
            pedido.AtualizarItem(itemAtualizado);

            // Assert
            var itemExistente = pedido.PedidoItems.FirstOrDefault(i => i.ProdutoId == produtoId);
            Assert.NotNull(itemExistente);
            Assert.Equal(2, itemExistente?.Quantidade);
            Assert.Equal(200m, itemExistente?.ValorUnitario);
            Assert.Equal("Produto Teste Atualizado", itemExistente?.ProdutoNome);
        }

        [Fact]
        public void AtualizarItem_DeveLancarExcecao_QuandoItemNaoExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var itemInexistente = new PedidoItem(Guid.NewGuid(), "Produto Inexistente", 1, 100m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => pedido.AtualizarItem(itemInexistente));
            Assert.Equal("O item não pertence ao pedido", exception.Message);
        }
        #endregion

        #region AtualizarUnidades
        [Fact]
        public void AtualizarUnidades_DeveAtualizarQuantidade_QuandoItemExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var itemOriginal = new PedidoItem(produtoId, "Produto Teste", 1, 100m);
            pedido.AdicionarItem(itemOriginal);

            var novaQuantidade = 5;

            // Act
            pedido.AtualizarUnidades(itemOriginal, novaQuantidade);

            // Assert
            var itemExistente = pedido.PedidoItems.FirstOrDefault(i => i.ProdutoId == produtoId);
            Assert.NotNull(itemExistente);
            Assert.Equal(novaQuantidade, itemExistente?.Quantidade);
        }

        [Fact]
        public void AtualizarUnidades_DeveLancarExcecao_QuandoItemNaoExiste()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var itemInexistente = new PedidoItem(Guid.NewGuid(), "Produto Inexistente", 1, 100m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => pedido.AtualizarUnidades(itemInexistente, 5));
            Assert.Equal("O item não pertence ao pedido", exception.Message);
        }
        #endregion

        #region TornarRascunho
        [Fact]
        public void TornarRascunho_DeveDefinirStatusComoRascunho()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            pedido.TornarRascunho();

            // Assert
            Assert.Equal(PedidoStatus.Rascunho, pedido.PedidoStatus);
        }

        [Fact]
        public void TornarRascunho_SeNaoEstiverEmRascunho_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();

            // Act
            try
            {
                pedido.TornarRascunho();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("O pedido já está em um status diferente de rascunho", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region IniciarPedido
        [Fact]
        public void IniciarPedido_DeveDefinirStatusComoIniciado()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            pedido.IniciarPedido();

            // Assert
            Assert.Equal(PedidoStatus.Iniciado, pedido.PedidoStatus);
        }

        [Fact]
        public void IniciarPedido_SeNaoEstiverEmRascunho_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();

            // Act
            try
            {
                pedido.IniciarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("O pedido não pode ser iniciado, pois não está em rascunho", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region ColocarPedidoComoPago
        [Fact]
        public void ColocarPedidoComoPago_DeveDefinirStatusComoPago()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();

            // Act
            pedido.ColocarPedidoComoPago();

            // Assert
            Assert.Equal(PedidoStatus.Pago, pedido.PedidoStatus);
        }

        [Fact]
        public void ColocarPedidoComoPago_SeNaoEstiverIniciado_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            try
            {
                pedido.ColocarPedidoComoPago();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser colocado como pago, pois o mesmo não está iniciado", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region CancelarPedido
        [Fact]
        public void CancelarPedido_DeveDefinirStatusComoCancelado()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            pedido.CancelarPedido();

            // Assert
            Assert.Equal(PedidoStatus.Cancelado, pedido.PedidoStatus);
        }

        [Fact]
        public void CancelarPedido_SeJaEstiverCancelado_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.CancelarPedido();

            // Act
            try
            {
                pedido.CancelarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido já está cancelado", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }

        [Fact]
        public void CancelarPedido_SeJaEstiverPronto_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();
            pedido.ColocarPedidoComoPronto();

            // Act
            try
            {
                pedido.CancelarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser cancelado, pois já está pronto", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }

        [Fact]
        public void CancelarPedido_SeJaEstiverEmPreparacao_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();

            // Act
            try
            {
                pedido.CancelarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser cancelado, pois já foi para preparação", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }

        [Fact]
        public void CancelarPedido_SeJaEstiverFinalizado_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();
            pedido.ColocarPedidoComoPronto();
            pedido.FinalizarPedido();

            // Act
            try
            {
                pedido.CancelarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido já foi finalizado", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region ColocarPedidoComoPronto
        [Fact]
        public void ColocarPedidoComoPronto_DeveDefinirStatusComoPronto()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();

            // Act
            pedido.ColocarPedidoComoPronto();

            // Assert
            Assert.Equal(PedidoStatus.Pronto, pedido.PedidoStatus);
        }

        [Fact]
        public void ColocarPedidoComoPronto_SeNaoEstiverEmPreparacao_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();

            // Act
            try
            {
                pedido.ColocarPedidoComoPronto();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser colocado como pronto, pois o mesmo não está em preparação", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region ColocarPedidoEmPreparacao
        [Fact]
        public void ColocarPedidoEmPreparacao_DeveDefinirStatusComoEmPreparacao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();

            // Act
            pedido.ColocarPedidoEmPreparacao();

            // Assert
            Assert.Equal(PedidoStatus.EmPreparacao, pedido.PedidoStatus);
        }

        [Fact]
        public void ColocarPedidoEmPreparacao_SeNaoEstiverRecebido_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            try
            {
                pedido.ColocarPedidoEmPreparacao();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser colocado em preparação, pois o mesmo não foi recebido", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region ColocarPedidoComoRecebido
        [Fact]
        public void ColocarPedidoComoRecebido_DeveDefinirStatusComoRecebido()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();

            // Act
            pedido.ColocarPedidoComoRecebido();

            // Assert
            Assert.Equal(PedidoStatus.Recebido, pedido.PedidoStatus);
        }

        [Fact]
        public void ColocarPedidoComoRecebido_SeNaoEstiverPago_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            try
            {
                pedido.ColocarPedidoComoRecebido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser colocado como recebido, pois o mesmo não foi pago", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        #region FinalizarPedido
        [Fact]
        public void FinalizarPedido_DeveDefinirStatusComoFinalizado()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();
            pedido.ColocarPedidoComoPronto();

            // Act
            pedido.FinalizarPedido();

            // Assert
            Assert.Equal(PedidoStatus.Finalizado, pedido.PedidoStatus);
        }

        [Fact]
        public void FinalizarPedido_SeNaoEstiverPronto_DeveLancarExcecao()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            // Act
            try
            {
                pedido.FinalizarPedido();
            }
            catch (DomainException ex)
            {
                // Assert
                Assert.Equal("Pedido não pode ser finalizado, pois não está pronto", ex.Message);
                return;
            }

            Assert.True(false, "Deveria ter lançado exceção");
        }
        #endregion

        [Theory]
        [InlineData(PedidoStatus.Rascunho)]
        [InlineData(PedidoStatus.Iniciado)]
        [InlineData(PedidoStatus.Pago)]
        [InlineData(PedidoStatus.Cancelado)]
        [InlineData(PedidoStatus.Pronto)]
        [InlineData(PedidoStatus.EmPreparacao)]
        [InlineData(PedidoStatus.Recebido)]
        [InlineData(PedidoStatus.Finalizado)]
        public void AtualizarStatus_DeveDefinirStatusCorreto(PedidoStatus novoStatus)
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            switch (novoStatus)
            {
                case PedidoStatus.Pago:
                    pedido.IniciarPedido();
                    break;
                case PedidoStatus.Recebido:
                    pedido.IniciarPedido();
                    pedido.ColocarPedidoComoPago();
                    break;
                case PedidoStatus.EmPreparacao:
                    pedido.IniciarPedido();
                    pedido.ColocarPedidoComoPago();
                    pedido.ColocarPedidoComoRecebido();
                    break;
                case PedidoStatus.Pronto:
                    pedido.IniciarPedido();
                    pedido.ColocarPedidoComoPago();
                    pedido.ColocarPedidoComoRecebido();
                    pedido.ColocarPedidoEmPreparacao();
                    break;                
                case PedidoStatus.Finalizado:
                    pedido.IniciarPedido();
                    pedido.ColocarPedidoComoPago();
                    pedido.ColocarPedidoComoRecebido();
                    pedido.ColocarPedidoEmPreparacao();
                    pedido.ColocarPedidoComoPronto();
                    break;
                default:
                    break;
            }

            // Act
            pedido.AtualizarStatus(novoStatus);

            // Assert
            Assert.Equal(novoStatus, pedido.PedidoStatus);
        }

        [Fact]
        public void AtualizarStatus_DeveLancarExcecaoParaStatusInvalido()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var statusInvalido = (PedidoStatus)999; // Um valor que não existe no enum

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => pedido.AtualizarStatus(statusInvalido));
            Assert.Equal("Status do pedido inválido", exception.Message);
        }
    }
}
