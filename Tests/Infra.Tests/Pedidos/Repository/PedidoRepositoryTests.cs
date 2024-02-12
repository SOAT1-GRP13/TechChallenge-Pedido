using Infra.Pedidos.Repository;
using Infra.Pedidos;
using Microsoft.EntityFrameworkCore;
using Moq;
using Domain.Pedidos;
using Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Infra.Tests.Pedidos.Repository
{
    public class PedidoRepositoryTests
    {
        private readonly Mock<IOptions<Secrets>> _mockOptions;
        private readonly Secrets _secrets;

        public PedidoRepositoryTests()
        {
            _mockOptions = new Mock<IOptions<Secrets>>();
            _secrets = new Secrets ();

            _mockOptions.Setup(opt => opt.Value).Returns(_secrets);
        }

        #region testes ObterPorId
        [Fact]
        public async Task ObterPorId_DeveRetornarPedido_QuandoPedidoExiste()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repository = new PedidoRepository(dbContext);

            var pedido = new Pedido(Guid.NewGuid(), 100);
            dbContext.Pedidos.Add(pedido);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.ObterPorId(pedido.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pedido.Id, result?.Id);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNull_QuandoPedidoNaoExiste()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repository = new PedidoRepository(dbContext);

            var idInexistente = Guid.NewGuid();

            // Act
            var result = await repository.ObterPorId(idInexistente);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region testes ObterListaPorClienteId
        [Fact]
        public async Task ObterListaPorClienteId_DeveRetornarPedidos_QuandoExistemPedidos()
        {
            var context = CreateDbContext();

            var clienteId = Guid.NewGuid();
            var pedidos = new List<Pedido>
            {
                new Pedido(clienteId, 100),
                new Pedido(clienteId, 200),
            };

            context.Pedidos.AddRange(pedidos);
            context.SaveChanges();

            var repository = new PedidoRepository(context);

            // Ação
            var resultado = await repository.ObterListaPorClienteId(clienteId);

            // Assertiva
            Assert.NotNull(resultado);
            Assert.Equal(pedidos.Count, resultado.Count());
        }
        #endregion

        #region testes ObterPedidoRascunhoPorClienteId
        [Fact]
        public async Task ObterPedidoRascunhoPorClienteId_DeveRetornarPedido_QuandoExistePedidoRascunho()
        {
            var context = CreateDbContext();

            var clienteId = Guid.NewGuid();
            var pedidoRascunho = new Pedido(clienteId, 100);
            pedidoRascunho.TornarRascunho();
            var pedidoPago = new Pedido(clienteId, 200);
            pedidoPago.ColocarPedidoComoPago();

            context.Pedidos.Add(pedidoRascunho);
            context.Pedidos.Add(pedidoPago);

            context.SaveChanges();

            var repository = new PedidoRepository(context);

            // Ação
            var pedidoEncontrado = await repository.ObterPedidoRascunhoPorClienteId(clienteId);

            // Assertiva
            Assert.NotNull(pedidoEncontrado);
            Assert.Equal(PedidoStatus.Rascunho, pedidoEncontrado?.PedidoStatus);
        }

        [Fact]
        public async Task ObterPedidoRascunhoPorClienteId_DeveRetornarNull_QuandoNaoExistemPedidosRascunho()
        {
            var context = CreateDbContext();

            var clienteId = Guid.NewGuid();

            var pedido = new Pedido(clienteId, 100);
            pedido.ColocarPedidoComoPago();
            context.Pedidos.Add(pedido);

            context.SaveChanges();

            var repository = new PedidoRepository(context);

            // Ação
            var pedidoEncontrado = await repository.ObterPedidoRascunhoPorClienteId(clienteId);

            // Assertiva
            Assert.Null(pedidoEncontrado);
        }
        #endregion

        #region testes Adicionar
        [Fact]
        public async Task Adicionar_DeveAdicionarPedido_QuandoPedidoEhValido()
        {
            var context = CreateDbContext();

            var clienteId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 100);
            var repository = new PedidoRepository(context);

            // Ação
            repository.Adicionar(pedido);
            await context.SaveChangesAsync();

            // Assertiva
            var pedidoAdicionado = await context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedido.Id);
            Assert.NotNull(pedidoAdicionado);
            Assert.Equal(clienteId, pedidoAdicionado?.ClienteId);
        }
        #endregion

        #region testes Atualizar
        [Fact]
        public async Task Atualizar_DeveAtualizarPedido_QuandoPedidoExiste()
        {
            var context = CreateDbContext();

            var clienteId = Guid.NewGuid();
            var pedido = new Pedido(clienteId, 100);
            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            // Atualizar o pedido
            var repository = new PedidoRepository(context);
            pedido.ColocarPedidoComoPago();
            repository.Atualizar(pedido);
            await context.SaveChangesAsync();

            // Assertiva
            var pedidoAtualizado = await context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedido.Id);
            Assert.NotNull(pedidoAtualizado);
            Assert.Equal(PedidoStatus.Pago, pedidoAtualizado?.PedidoStatus);
        }
        #endregion

        #region testes ObterItemPorId
        [Fact]
        public async Task ObterItemPorId_DeveRetornarItem_QuandoItemExiste()
        {
            var context = CreateDbContext();

            var pedido = new Pedido(Guid.NewGuid(), 100);
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 50);
            pedido.AdicionarItem(pedidoItem);
            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            // Obter o item por ID
            var repository = new PedidoRepository(context);
            var itemObtido = await repository.ObterItemPorId(pedidoItem.Id);

            // Assertiva
            Assert.NotNull(itemObtido);
            Assert.Equal(pedidoItem.Id, itemObtido?.Id);
        }
        #endregion

        #region testes AdicionarItem
        [Fact]
        public async Task AdicionarItem_DeveAdicionarItemAoPedido()
        {
            var context = CreateDbContext();

            var pedido = new Pedido(Guid.NewGuid(), 100);
            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 50);
            pedido.AdicionarItem(pedidoItem);

            var repository = new PedidoRepository(context);
            repository.AdicionarItem(pedidoItem);
            await context.SaveChangesAsync();

            var itemAdicionado = await context.PedidoItems.FirstOrDefaultAsync(i => i.Id == pedidoItem.Id);

            Assert.NotNull(itemAdicionado);
            Assert.Equal(pedidoItem.Id, itemAdicionado?.Id);
            Assert.Equal(pedidoItem.Quantidade, itemAdicionado?.Quantidade);
        }
        #endregion

        #region testes AtualizarItem
        [Fact]
        public async Task AtualizarItem_DeveAtualizarItemExistente()
        {
            var context = CreateDbContext();

            var pedido = new Pedido(Guid.NewGuid(), 100);
            pedido.TornarRascunho();
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 50);
            pedido.AdicionarItem(pedidoItem);

            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            pedido.ColocarPedidoComoPago();
            var repository = new PedidoRepository(context);
            repository.AtualizarItem(pedidoItem);
            await context.SaveChangesAsync();

            var itemAtualizado = await context.PedidoItems.FirstOrDefaultAsync(i => i.Id == pedidoItem.Id);

            Assert.NotNull(itemAtualizado);
            Assert.Equal(pedidoItem.Quantidade, itemAtualizado?.Quantidade);
            Assert.Equal(pedido.PedidoStatus, itemAtualizado?.Pedido?.PedidoStatus);
        }
        #endregion

        #region testes RemoverItem
        [Fact]
        public async Task RemoverItem_DeveRemoverItemExistente()
        {
            var context = CreateDbContext();

            var pedido = new Pedido(Guid.NewGuid(), 100);
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 50);
            pedido.AdicionarItem(pedidoItem);

            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            var repository = new PedidoRepository(context);
            repository.RemoverItem(pedidoItem);
            await context.SaveChangesAsync();

            var itemRemovido = await context.PedidoItems.FirstOrDefaultAsync(i => i.Id == pedidoItem.Id);

            Assert.Null(itemRemovido);
        }
        #endregion

        #region testes Dispose
        [Fact]
        public void Dispose_DeveChamarDisposeDoContexto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PedidosContext>()
                .UseInMemoryDatabase(databaseName: "TestePedidoDbDispose")
                .Options;

            var mockContext = new Mock<PedidosContext>(options);
            var repository = new PedidoRepository(mockContext.Object);

            // Act
            repository.Dispose();

            // Assert
            mockContext.Verify(x => x.Dispose(), Times.Once());
        }
        #endregion

        #region metodos privados
        public static PedidosContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PedidosContext>()
                .UseInMemoryDatabase(databaseName: "TestePedidoDbDispose")
                .Options;

            var dbContext = new PedidosContext(options);

            return dbContext;
        }
        #endregion
    }
}
