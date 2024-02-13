using Application.Pedidos.Queries.DTO;
using Application.Pedidos.UseCases;
using AutoMapper;
using Domain.Base.Data;
using Domain.Base.DomainObjects;
using Domain.Pedidos;
using Moq;

namespace Application.Tests.Pedidos.UseCases
{
    public class PedidoUseCaseTests
    {
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PedidoUseCase _pedidoUseCase;

        public PedidoUseCaseTests()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _pedidoUseCase = new PedidoUseCase(_pedidoRepositoryMock.Object, _mapperMock.Object);

            _pedidoRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        }

        #region AdicionarItem
        [Fact]
        public async Task AdicionarItem_DeveCriarNovoPedido_QuandoPedidoNaoExiste()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            string nomeProduto = "Produto Teste";
            int quantidade = 5;
            decimal valorUnitario = 100m;

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorClienteId(clienteId));

            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _pedidoUseCase.AdicionarItem(clienteId, produtoId, nomeProduto, quantidade, valorUnitario);

            // Assert
            Assert.True(resultado);
            _pedidoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Pedido>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once());
        }
        #endregion

        #region AtualizarItem
        [Fact]
        public async Task AtualizarItem_DeveAtualizarItem_QuandoPedidoEItemExistem()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            int novaQuantidade = 10;
            var pedidoExistente = new Pedido(clienteId, 100);
            var pedidoItemExistente = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            pedidoExistente.AdicionarItem(pedidoItemExistente);
            pedidoExistente.TornarRascunho();

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorClienteId(clienteId)).ReturnsAsync(pedidoExistente);
            _pedidoRepositoryMock.Setup(r => r.ObterItemPorPedido(pedidoExistente.Id, produtoId)).ReturnsAsync(pedidoItemExistente);

            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _pedidoUseCase.AtualizarItem(clienteId, produtoId, novaQuantidade);

            // Assert
            Assert.True(resultado);
            _pedidoRepositoryMock.Verify(r => r.AtualizarItem(It.IsAny<PedidoItem>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once());
        }

        [Fact]
        public async Task AtualizarItem_DeveLancarDomainException_QuandoPedidoOuItemNaoExistem()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            int novaQuantidade = 10;

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorClienteId(clienteId));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.AtualizarItem(clienteId, produtoId, novaQuantidade));
        }
        #endregion

        #region RemoverItem
        [Fact]
        public async Task RemoverItem_DeveRemoverItem_QuandoPedidoEItemExistem()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 100);
            var itemPedido = new PedidoItem(produtoId, "Produto Teste", 1, 100);
            pedido.AdicionarItem(itemPedido);

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorClienteId(clienteId)).ReturnsAsync(pedido);
            _pedidoRepositoryMock.Setup(r => r.ObterItemPorPedido(pedido.Id, produtoId)).ReturnsAsync(itemPedido);

            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _pedidoUseCase.RemoverItem(clienteId, produtoId);

            // Assert
            Assert.True(resultado);
            _pedidoRepositoryMock.Verify(r => r.RemoverItem(It.IsAny<PedidoItem>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once());
        }

        [Fact]
        public async Task RemoverItem_DeveLancarDomainException_QuandoPedidoOuItemNaoExistem()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            _pedidoRepositoryMock.Setup(r => r.ObterPedidoRascunhoPorClienteId(clienteId));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.RemoverItem(clienteId, produtoId));
        }
        #endregion

        #region TrocaStatusPedido
        [Fact]
        public async Task TrocaStatusPedido_DeveRetornarPedidoAtualizado_QuandoPedidoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedido = new Pedido(pedidoId, 100);
            pedido.IniciarPedido();

            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);

            var pedidoDto = new PedidoDto();
            _mapperMock.Setup(m => m.Map<PedidoDto>(pedido)).Returns(pedidoDto);

            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _pedidoUseCase.TrocaStatusPedido(pedidoId, PedidoStatus.Pago);

            // Assert
            Assert.Equal(pedidoDto, resultado);
            _pedidoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Pedido>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once());
        }

        [Fact]
        public async Task TrocaStatusPedido_DeveRetornarPedidoDtoVazio_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId));

            // Act
            var resultado = await _pedidoUseCase.TrocaStatusPedido(pedidoId, PedidoStatus.Pago);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(Guid.Empty, resultado.Id);
        }

        [Fact]
        public async Task TrocaStatusPedido_DeveLancarDomainException_QuandoStatusInvalido()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedido = new Pedido(pedidoId, 100);
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.TrocaStatusPedido(pedidoId, PedidoStatus.Iniciado));
        }
        #endregion

        #region IniciarPedido
        [Fact]
        public async Task IniciarPedido_DeveLancarDomainException_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.IniciarPedido(pedidoId));
        }
        #endregion

        #region FinalizarPedido
        [Fact]
        public async Task FinalizarPedido_DeveFinalizarPedido_QuandoPedidoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedido = new Pedido(pedidoId, 100);
            pedido.IniciarPedido();
            pedido.ColocarPedidoComoPago();
            pedido.ColocarPedidoComoRecebido();
            pedido.ColocarPedidoEmPreparacao();
            pedido.ColocarPedidoComoPronto();

            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);

            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var resultado = await _pedidoUseCase.FinalizarPedido(pedidoId);

            // Assert
            Assert.True(resultado);
            Assert.Equal(PedidoStatus.Finalizado, pedido.PedidoStatus);
            _pedidoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Pedido>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once());
        }

        [Fact]
        public async Task FinalizarPedido_DeveLancarDomainException_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.FinalizarPedido(pedidoId));
        }
        #endregion

        #region CancelarProcessamento
        [Fact]
        public async Task CancelarProcessamento_DeveLancarDomainException_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _pedidoUseCase.CancelarProcessamento(pedidoId));
        }
        #endregion

        #region ObterPedidoPorId
        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarPedidoDto_QuandoPedidoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedido = new Pedido(pedidoId, 100);
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);

            var pedidoDto = new PedidoDto { Id = pedidoId };
            _mapperMock.Setup(m => m.Map<PedidoDto>(pedido)).Returns(pedidoDto);

            // Act
            var resultado = await _pedidoUseCase.ObterPedidoPorId(pedidoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pedidoId, resultado.Id);
            _mapperMock.Verify(m => m.Map<PedidoDto>(pedido), Times.Once());
        }

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarNull_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId));

            // Act
            var resultado = await _pedidoUseCase.ObterPedidoPorId(pedidoId);

            // Assert
            Assert.Null(resultado);
        }
        #endregion

        #region Dispose

        [Fact]
        public void Dispose_DeveChamarDisposeNoPedidoRepository()
        {
            // Act
            _pedidoUseCase.Dispose();

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Dispose(), Times.Once());
        }
        #endregion
    }
}
