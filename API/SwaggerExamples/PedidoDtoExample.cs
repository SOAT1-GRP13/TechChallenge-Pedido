using Application.Pedidos.Queries.DTO;
using Domain.Pedidos;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace API.SwaggerExamples
{
    [ExcludeFromCodeCoverage]
    public class PedidoDtoExample : IMultipleExamplesProvider<IEnumerable<PedidoDto>>
    {
        public IEnumerable<SwaggerExample<IEnumerable<PedidoDto>>> GetExamples()
        {
            yield return SwaggerExample.Create<IEnumerable<PedidoDto>>("Lista de Pedidos", new[]
            {
                new PedidoDto
                {
                    Codigo = 1,
                    DataCadastro = DateTime.UtcNow.AddDays(-7),
                    Id =  Guid.NewGuid(),
                    ValorTotal = 39.9m,
                    PedidoStatus = PedidoStatus.Pago
                }
            });
        }
    }
}
