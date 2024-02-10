using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Application.Pedidos.Boundaries
{
    public class AtualizarItemInput
    {
        [Required]
        [SwaggerSchema(
            Title = "Guid do produto",
            Format = "Guid")]
        public Guid Id { get; set; }

        [Required]
        [SwaggerSchema(
            Title = "Nome do produto",
            Format = "string")]
        public string Nome { get; set; }

        [Required]
        [SwaggerSchema(
            Title = "Valor",
            Format = "decimal")]
        public decimal Valor { get; set; }

        [Required]
        [SwaggerSchema(
            Title = "Quantidade",
            Format = "int")]
        public int Quantidade { get; set; }
    }
}
