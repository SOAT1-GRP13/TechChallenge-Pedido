﻿using Moq;
using Domain.Pedidos;
using Domain.Base.DomainObjects;
using Application.Pedidos.UseCases;
using Application.Pedidos.Commands;
using Application.Pedidos.Handlers;
using Application.Pedidos.Boundaries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Domain.Base.Messages.CommonMessages.Notifications;

namespace Application.Tests.Pedidos.Handlers
{
    public class AtualizarStatusPedidoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarPedidoDto_QuandoPedidoAtualizadoComSucesso()
        {
            // Arrange
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var pedidoUseCaseMock = new Mock<IPedidoUseCase>();

            var guid = Guid.NewGuid();

            var pedidoDto = new PedidoDto
            {
                Id = guid,
                Codigo = 1,
                PedidoStatus = PedidoStatus.EmPreparacao
            };
            var command = new AtualizarStatusPedidoCommand(new AtualizarStatusPedidoInput(guid, (int)PedidoStatus.EmPreparacao));

            pedidoUseCaseMock.Setup(p => p.TrocaStatusPedido(guid, PedidoStatus.EmPreparacao)).ReturnsAsync(pedidoDto);

            var handler = new AtualizarStatusPedidoCommandHandler(pedidoUseCaseMock.Object, mediatorHandlerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_DevePublicarNotificacao_QuandoPedidoInvalido()
        {
            // Arrange
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var pedidoUseCaseMock = new Mock<IPedidoUseCase>();

            // Criando um comando com ID de pedido inválido (Guid vazio)
            var command = new AtualizarStatusPedidoCommand(new AtualizarStatusPedidoInput(Guid.Empty, (int)PedidoStatus.EmPreparacao));

            var handler = new AtualizarStatusPedidoCommandHandler(pedidoUseCaseMock.Object, mediatorHandlerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mediatorHandlerMock.Verify(m => m.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.AtLeastOnce());
            Assert.False(command.EhValido());
        }

        [Fact]
        public async Task Handle_DevePublicarNotificacao_QuandoDomainExceptionLancada()
        {
            // Arrange
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var pedidoUseCaseMock = new Mock<IPedidoUseCase>();

            var guid = Guid.NewGuid();
            var command = new AtualizarStatusPedidoCommand(new AtualizarStatusPedidoInput(guid, (int)PedidoStatus.EmPreparacao));

            pedidoUseCaseMock.Setup(p => p.TrocaStatusPedido(guid, PedidoStatus.EmPreparacao))
                .ThrowsAsync(new DomainException("Erro de domínio simulado"));

            var handler = new AtualizarStatusPedidoCommandHandler(pedidoUseCaseMock.Object, mediatorHandlerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mediatorHandlerMock.Verify(m => m.PublicarNotificacao(It.Is<DomainNotification>(dn => dn.Value == "Erro de domínio simulado")), Times.Once());
        }

        [Fact]
        public async Task Handle_DevePublicarNotificacaoERetornarFalse_QuandoPedidoNaoAtualizado()
        {
            // Arrange
            var mediatorHandlerMock = new Mock<IMediatorHandler>();
            var pedidoUseCaseMock = new Mock<IPedidoUseCase>();

            var guid = Guid.NewGuid();

            var pedidoDto = new PedidoDto
            {
                Id = guid,
                Codigo = 0,
                PedidoStatus = PedidoStatus.EmPreparacao
            };
            var command = new AtualizarStatusPedidoCommand(new AtualizarStatusPedidoInput(guid, (int)PedidoStatus.EmPreparacao));

            pedidoUseCaseMock.Setup(p => p.TrocaStatusPedido(guid, PedidoStatus.EmPreparacao)).ReturnsAsync(pedidoDto);

            var handler = new AtualizarStatusPedidoCommandHandler(pedidoUseCaseMock.Object, mediatorHandlerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            // Verifica se uma notificação com a mensagem "Pedido não encontrado" foi publicada
            mediatorHandlerMock.Verify(m => m.PublicarNotificacao(It.Is<DomainNotification>(dn => dn.Value == "Pedido não encontrado")), Times.Once());

            // Verifica se o resultado é null
            Assert.False(result);
        }
    }
}
