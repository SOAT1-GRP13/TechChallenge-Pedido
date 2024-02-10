using Application.Pedidos.Commands;
using Domain.Catalogo;
using Domain.Pedidos;
using Polly;
using MediatR;
using System.Net;
using Infra.Pedidos;
using Polly.Extensions.Http;
using Infra.Pedidos.Repository;
using Infra.Catalogo.Repository;
using Application.Pedidos.Queries;
using Application.Pedidos.Handlers;
using Application.Pedidos.UseCases;
using Application.Catalogo.Queries;
using Application.Pedidos.Queries.DTO;
using Domain.Base.Communication.Mediator;
using Domain.Base.Messages.CommonMessages.Notifications;

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

            // Pedidos
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoQueries, PedidoQueries>();
            services.AddScoped<IPedidoUseCase, PedidoUseCase>();
            services.AddScoped<PedidosContext>();

            services.AddScoped<IRequestHandler<AdicionarItemPedidoCommand, bool>, AdicionarItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarItemPedidoCommand, bool>, AtualizarItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarStatusPedidoCommand, bool>, AtualizarStatusPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoverItemPedidoCommand, bool>, RemoverItemPedidoCommandHandler>();
            services.AddScoped<IRequestHandler<IniciarPedidoCommand, CarrinhoDto>, IniciarPedidoCommandHandler>();
        }
    }
}
