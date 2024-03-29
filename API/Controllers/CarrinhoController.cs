﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Pedidos.Queries;
using Application.Pedidos.Commands;
using Application.Pedidos.Queries.DTO;
using Application.Pedidos.Boundaries;
using Microsoft.AspNetCore.Authorization;
using Domain.Base.Communication.Mediator;
using Swashbuckle.AspNetCore.Annotations;
using Domain.Base.Messages.CommonMessages.Notifications;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [SwaggerTag("Endpoints relacionados ao carrinho, sendo necessário se autenticar e o clienteId é pego de forma automatica")]
    public class CarrinhoController : ControllerBase
    {
        private readonly IPedidoQueries _pedidoQueries;
        private readonly IMediatorHandler _mediatorHandler;

        public CarrinhoController(INotificationHandler<DomainNotification> notifications,
                                  IMediatorHandler mediatorHandler,
                                  IPedidoQueries pedidoQueries) : base(notifications, mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
            _pedidoQueries = pedidoQueries;
        }


        [HttpPost("adicionar-item")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Adicionar Item ao carrinho",
            Description = "Adiciona o item desejado ao carrinho")]
        [SwaggerResponse(200, "Retorna dados do carrinho", typeof(CarrinhoDto))]
        [SwaggerResponse(404, "Caso não encontre o produto com o Id informado")]
        [SwaggerResponse(400, "Caso não obedeça alguma regra de negocio", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        public async Task<IActionResult> AdicionarItem([FromBody] AdicionarItemInput input)
        {
            try
            {
                var command = new AdicionarItemPedidoCommand(ObterClienteId(), input.Id, input.Nome, input.Quantidade, input.Valor);
                await _mediatorHandler.EnviarComando<AdicionarItemPedidoCommand, bool>(command);

                if (!OperacaoValida())
                    return StatusCode(StatusCodes.Status400BadRequest, ObterMensagensErro());

                return Ok(await _pedidoQueries.ObterCarrinhoCliente(ObterClienteId()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,

                                       $"Erro ao tentar adicionar item ao carrinho. Erro: {ex.Message}");
            }

        }

        [HttpPut("atualizar-item")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Atualizar item do carrinho",
            Description = "Atualiza o item desejado no carrinho")]
        [SwaggerResponse(200, "Retorna dados do carrinho", typeof(CarrinhoDto))]
        [SwaggerResponse(404, "Caso não encontre o produto com o Id informado")]
        [SwaggerResponse(400, "Caso não obedeça alguma regra de negocio", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        public async Task<IActionResult> AtualizarItem([FromBody] AtualizarItemInput input)
        {
            try
            {
                var command = new AtualizarItemPedidoCommand(ObterClienteId(), input.Id, input.Quantidade);
                await _mediatorHandler.EnviarComando<AtualizarItemPedidoCommand, bool>(command);

                if (!OperacaoValida())
                    return StatusCode(StatusCodes.Status400BadRequest, ObterMensagensErro());

                return Ok(await _pedidoQueries.ObterCarrinhoCliente(ObterClienteId()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                       $"Erro ao tentar atualizar item do carrinho. Erro: {ex.Message}");
            }
        }

        [HttpDelete("remover-item/{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Remover item do carrinho",
            Description = "Remove o item desejado no carrinho")]
        [SwaggerResponse(200, "Retorna dados do carrinho", typeof(CarrinhoDto))]
        [SwaggerResponse(404, "Caso não encontre o produto com o Id informado")]
        [SwaggerResponse(400, "Caso não obedeça alguma regra de negocio", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        public async Task<IActionResult> RemoverItem([FromRoute] Guid id)
        {
            try
            {
                var command = new RemoverItemPedidoCommand(ObterClienteId(), id);
                await _mediatorHandler.EnviarComando<RemoverItemPedidoCommand, bool>(command);

                if (!OperacaoValida())
                    return StatusCode(StatusCodes.Status400BadRequest, ObterMensagensErro());

                return Ok(await _pedidoQueries.ObterCarrinhoCliente(ObterClienteId()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                       $"Erro ao tentar remover item do carrinho. Erro: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Listar itens do carrinho",
            Description = "Lista os itens no carrinho")]
        [SwaggerResponse(200, "Retorna dados do carrinho", typeof(CarrinhoDto))]
        [SwaggerResponse(404, "Caso não encontre nenhum carrinho")]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        [Route("meu-carrinho")]
        public async Task<IActionResult> MeuCarrinho()
        {
            try
            {
                var carrinho = await _pedidoQueries.ObterCarrinhoCliente(ObterClienteId());
                if (carrinho is null)
                    return StatusCode(StatusCodes.Status404NotFound, "Nenhum carrinho em rascunho encontrado");

                return Ok(carrinho);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                       $"Erro ao tentar recuperar carrinho. Erro: {ex.Message}");
            }
        }

        [HttpPost("confirmar-pedido")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Confirma o pedido",
            Description = "Confirma o pedido e é nesta etapa que deve integrar com o mercado pago trazendo o QR Code.")]
        [SwaggerResponse(200, "Retorna pedido confirmado", typeof(CarrinhoDto))]
        [SwaggerResponse(404, "Caso não encontre nenhum carrinho")]
        [SwaggerResponse(400, "Caso não obedeça alguma regra de negocio")]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        public async Task<IActionResult> ConfirmarPedido([FromBody] IniciarPedidoInput input)
        {
            //IniciarPedidoCommand Dispara todos os eventos de dominio para criar o pedido, realizar pagamento e finalizar pedido.
            var command = new IniciarPedidoCommand(input.PedidoId, ObterClienteId(), ObterClienteEmail());

            var pedido = await _mediatorHandler.EnviarComando<IniciarPedidoCommand, CarrinhoDto>(command);

            if (!OperacaoValida())
                return StatusCode(StatusCodes.Status400BadRequest, ObterMensagensErro());

            return Ok(pedido);
        }

        [HttpGet("consultar-status-pedido/{pedidoId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Consultar status do pedido",
            Description = "Consulta status do pedido a partir do Guid")]
        [SwaggerResponse(200, "Retorna o pedido atualizado", typeof(ConsultarStatusPedidoOutput))]
        [SwaggerResponse(404, "Caso não encontre o pedido com o Id informado")]
        [SwaggerResponse(500, "Caso algo inesperado aconteça")]
        public async Task<IActionResult> ConsultarStatusPedido([FromRoute] Guid pedidoId)
        {
            var command = new ConsultarStatusPedidoCommand(pedidoId);
            var pedido = await _mediatorHandler.EnviarComando<ConsultarStatusPedidoCommand, ConsultarStatusPedidoOutput>(command);

            if (!OperacaoValida())
                return StatusCode(StatusCodes.Status400BadRequest, ObterMensagensErro());

            return Ok(pedido);
        }
    }
}
