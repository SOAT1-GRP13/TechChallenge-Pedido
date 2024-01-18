using API.Controllers;
using Application.Catalogo.Dto;
using Application.Catalogo.Queries;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Commands;
using Application.Pedidos.Queries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Domain.Base.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace API.Tests.Controllers
{
    public class CarrinhoControllerTests
    {
        #region testes metodo AdicionarItem
        [Fact]
        public async Task AdicionarItem_DeveRetornarNotFound_QuandoProdutoNaoExiste()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();
            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();

            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((ProdutoDto)null);

            var input = new AdicionarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AdicionarItem(input);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AdicionarItem_DeveRetornarBadRequest_QuandoValidacaoFalha()
        {
            // Arrange
            var serviceProvider = new TestStartup().ConfigureServices(new ServiceCollection());

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = serviceProvider.GetRequiredService<IMediatorHandler>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandler, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());

            var input = new AdicionarItemInput { Id = Guid.NewGuid(), Quantidade = 0 };

            // Act
            var result = await controller.AdicionarItem(input);

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(result);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains("Id do produto inválido", mensagensErro);
            Assert.Contains("O nome do produto não foi informado", mensagensErro);
            Assert.Contains("A quantidade miníma de um item é 1", mensagensErro);
            Assert.Contains("O valor do item precisa ser maior que 0", mensagensErro);
        }

        [Fact]
        public async Task AdicionarItem_DeveRetornarOk_QuandoItemAdicionadoComSucesso()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());
            mediatorHandlerMock.Setup(m => m.EnviarComando<AdicionarItemPedidoCommand, bool>(It.IsAny<AdicionarItemPedidoCommand>())).ReturnsAsync(true);
            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ReturnsAsync(new CarrinhoDto());

            var input = new AdicionarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AdicionarItem(input);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AdicionarItem_DeveRetornarInternalServerError_QuandoExcecaoOcorrer()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ThrowsAsync(new Exception("Erro inesperado"));

            var input = new AdicionarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AdicionarItem(input);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }
        #endregion

        #region testes metodo AtualizarItem
        [Fact]
        public async Task AtualizarItem_DeveRetornarNotFound_QuandoProdutoNaoExiste()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();
            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();

            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((ProdutoDto)null);

            var input = new AtualizarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AtualizarItem(input);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AtualizarItem_DeveRetornarBadRequest_QuandoValidacaoFalha()
        {
            // Arrange
            var serviceProvider = new TestStartup().ConfigureServices(new ServiceCollection());

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = serviceProvider.GetRequiredService<IMediatorHandler>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandler, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());

            var input = new AtualizarItemInput { Id = Guid.NewGuid(), Quantidade = 0 };

            // Act
            var result = await controller.AtualizarItem(input);

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(result);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains("A quantidade miníma de um item é 1", mensagensErro);
        }

        [Fact]
        public async Task AtualizarItem_DeveRetornarOk_QuandoItemAdicionadoComSucesso()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());
            mediatorHandlerMock.Setup(m => m.EnviarComando<AtualizarItemPedidoCommand, bool>(It.IsAny<AtualizarItemPedidoCommand>())).ReturnsAsync(true);
            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ReturnsAsync(new CarrinhoDto());

            var input = new AtualizarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AtualizarItem(input);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AtualizarItem_DeveRetornarInternalServerError_QuandoExcecaoOcorrer()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ThrowsAsync(new Exception("Erro inesperado"));

            var input = new AtualizarItemInput { Id = Guid.NewGuid(), Quantidade = 1 };

            // Act
            var result = await controller.AtualizarItem(input);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }
        #endregion

        #region testes metodo RemoverItem
        [Fact]
        public async Task RemoverItem_DeveRetornarNotFound_QuandoProdutoNaoExiste()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();
            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();

            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((ProdutoDto)null);

            var guid = Guid.NewGuid();

            // Act
            var result = await controller.RemoverItem(guid);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoverItem_DeveRetornarBadRequest_QuandoValidacaoFalha()
        {
            // Arrange
            var serviceProvider = new TestStartup().ConfigureServices(new ServiceCollection());

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = serviceProvider.GetRequiredService<IMediatorHandler>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandler, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());

            var guid = Guid.Empty;

            // Act
            var result = await controller.RemoverItem(guid);

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(result);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains("Id do produto inválido", mensagensErro);
        }

        [Fact]
        public async Task RemoverItem_DeveRetornarOk_QuandoItemAdicionadoComSucesso()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new ProdutoDto());
            mediatorHandlerMock.Setup(m => m.EnviarComando<RemoverItemPedidoCommand, bool>(It.IsAny<RemoverItemPedidoCommand>())).ReturnsAsync(true);
            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ReturnsAsync(new CarrinhoDto());

            var guid = Guid.NewGuid();

            // Act
            var result = await controller.RemoverItem(guid);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RemoverItem_DeveRetornarInternalServerError_QuandoExcecaoOcorrer()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            produtosQueriesMock.Setup(p => p.ObterPorId(It.IsAny<Guid>())).ThrowsAsync(new Exception("Erro inesperado"));

            var guid = Guid.NewGuid();

            // Act
            var result = await controller.RemoverItem(guid);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }
        #endregion

        #region testes metodo MeuCarrinho
        [Fact]
        public async Task MeuCarrinho_DeveRetornarOk_QuandoCarrinhoEncontrado()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ReturnsAsync(new CarrinhoDto());

            // Act
            var result = await controller.MeuCarrinho();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task MeuCarrinho_DeveRetornarNotFound_QuandoCarrinhoNaoEncontrado()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var notificationsMock = new Mock<INotificationHandler<DomainNotification>>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ReturnsAsync((CarrinhoDto)null);

            // Act
            var result = await controller.MeuCarrinho();

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task MeuCarrinho_DeveRetornarInternalServerError_QuandoExcecaoOcorrer()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var produtosQueriesMock = new Mock<IProdutosQueries>();
            var pedidoQueriesMock = new Mock<IPedidoQueries>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var notificationsMock = new Mock<INotificationHandler<DomainNotification>>();

            var controller = new CarrinhoController(domainNotificationHandler, produtosQueriesMock.Object, mediatorHandlerMock.Object, pedidoQueriesMock.Object);
            pedidoQueriesMock.Setup(p => p.ObterCarrinhoCliente(It.IsAny<Guid>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await controller.MeuCarrinho();

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }

        #endregion

        #region testes metodo ConfirmarPedido
        [Fact]
        public async Task ConfirmarPedido_DeveRetornarOk_QuandoPedidoConfirmadoComSucesso()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandlerMock = new Mock<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, null, mediatorHandlerMock.Object, null);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            var pedidoId = Guid.NewGuid();
            var iniciarPedidoCommand = new IniciarPedidoCommand(pedidoId, Guid.NewGuid());
            var confirmarPedidoOutput = new ConfirmarPedidoOutput("qrData", pedidoId);

            mediatorHandlerMock.Setup(m => m.EnviarComando<IniciarPedidoCommand, ConfirmarPedidoOutput>(It.IsAny<IniciarPedidoCommand>()))
                               .ReturnsAsync(confirmarPedidoOutput);

            // Act
            var result = await controller.ConfirmarPedido(new IniciarPedidoInput { PedidoId = iniciarPedidoCommand.PedidoId });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<ConfirmarPedidoOutput>(okResult.Value);
            Assert.Equal(confirmarPedidoOutput.QrData, returnedValue.QrData);
            Assert.Equal(confirmarPedidoOutput.PedidoId, returnedValue.PedidoId);
        }

        [Fact]
        public async Task ConfirmarPedido_DeveRetornarBadRequest_QuandoRegrasDeNegocioFalharem()
        {
            // Arrange
            var serviceProvider = new TestStartup().ConfigureServices(new ServiceCollection());

            var domainNotificationHandler = serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = serviceProvider.GetRequiredService<IMediatorHandler>();

            var controller = new CarrinhoController(domainNotificationHandler, null, mediatorHandler, null);
            var defaultHttpContext = new DefaultHttpContext { User = ClaimsPrincipal() };
            controller.ControllerContext = new ControllerContext { HttpContext = defaultHttpContext };

            var iniciarPedidoCommand = new IniciarPedidoCommand(Guid.Empty, Guid.Empty);

            // Act
            var result = await controller.ConfirmarPedido(new IniciarPedidoInput { PedidoId = iniciarPedidoCommand.PedidoId });

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(result);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains("Id do pedido inválido", mensagensErro);
        }
        #endregion

        #region metodo privados
        private ClaimsPrincipal ClaimsPrincipal()
        {
            var fakeUserId = Guid.NewGuid().ToString();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, fakeUserId) };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            return claimsPrincipal;
        }
        #endregion
    }
}
