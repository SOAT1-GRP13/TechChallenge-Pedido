using Application.Pedidos.Boundaries;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace API.SwaggerExamples
{
    [ExcludeFromCodeCoverage]
    public class AdicionarItemInputExample : IExamplesProvider<AdicionarItemInput>
    {
        public AdicionarItemInput GetExamples()
        {
            return new AdicionarItemInput
            {
                Id = new Guid("903562cf-1368-4e93-9de3-93f88b1407be"),
                Nome = "Coca-Cola - Lata",
                Valor = 5,
                Quantidade = 2,

            };
        }
    }
}
