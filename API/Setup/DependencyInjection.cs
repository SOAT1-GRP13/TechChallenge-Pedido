﻿using Application.Pedidos.Commands;
using Domain.Catalogo;
using Domain.Pedidos;
using Infra.Catalogo;
using Infra.Catalogo.Repository;
using Infra.Pedidos.Repository;
using Infra.Pedidos;
using MediatR;
using Domain.Base.Communication.Mediator;
using Domain.Base.Messages.CommonMessages.Notifications;
using Application.Pedidos.Queries;
using Application.Pedidos.Handlers;
using Application.Pedidos.Boundaries;
using Application.Pedidos.UseCases;
using Application.Catalogo.Queries;
using Application.Catalogo.Commands;
using Application.Catalogo.Boundaries;
using Application.Catalogo.Handlers;
using Polly.Extensions.Http;
using System.Net;
using Polly;

namespace API.Setup
{
    public static class DependencyInjection
    { 
        public static void RegisterServices(this IServiceCollection services)
        {
            //Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            //Domain Notifications 
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Catalogo
            services.AddHttpClient<IProdutoRepository, ProdutoRepository>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                        .WaitAndRetryAsync(2, retryAttempts => TimeSpan.FromSeconds(Math.Pow(2, retryAttempts)))
            );

            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutosQueries, ProdutosQueries>();
            services.AddScoped<IRequestHandler<AdicionarProdutoCommand, ProdutoOutput>, AdicionarProdutoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarProdutoCommand, ProdutoOutput>, AtualizarProdutoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoverProdutoCommand, bool>, RemoverProdutoCommandHandler>();
            services.AddScoped<CatalogoContext>();

            // Pedidos
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoQueries, PedidoQueries>();
            services.AddScoped<IPedidoUseCase, PedidoUseCase>();
            services.AddScoped<PedidosContext>();

            services.AddScoped<IRequestHandler<AtualizarStatusPedidoCommand, PedidoOutput>, AtualizarStatusPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AdicionarItemPedidoCommand, bool>, AdicionarItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarItemPedidoCommand, bool>, AtualizarItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoverItemPedidoCommand, bool>, RemoverItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<IniciarPedidoCommand, ConfirmarPedidoOutput>, IniciarPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<FinalizarPedidoCommand, bool>, FinalizarPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<CancelarProcessamentoPedidoCommand, bool>, CancelarProcessamentoPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<ConsultarStatusPedidoCommand, ConsultarStatusPedidoOutput>, ConsultarStatusPedidoCommandHandler>();
        }
    }
}
