﻿using Domain.Base.Data;

namespace Domain.Pedidos
{
    public interface IPedidoRepository : IRepository
    {
        Task<Pedido?> ObterPorId(Guid id);
        Task<IEnumerable<Pedido>> ObterListaPorClienteId(Guid clienteId);
        Task<Pedido?> ObterPedidoRascunhoPorClienteId(Guid clienteId);
        void Adicionar(Pedido pedido);
        void Atualizar(Pedido pedido);

        Task<PedidoItem?> ObterItemPorId(Guid id);
        Task<PedidoItem?> ObterItemPorPedido(Guid pedidoId, Guid produtoId);
        void AdicionarItem(PedidoItem pedidoItem);
        void AtualizarItem(PedidoItem pedidoItem);
        void RemoverItem(PedidoItem pedidoItem);
    }
}
