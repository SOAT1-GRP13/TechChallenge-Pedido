using AutoMapper;
using Domain.Pedidos;
using Application.Pedidos.Queries.DTO;

namespace Application.Pedidos.AutoMapper
{
    public class PedidosMappingProfile : Profile
    {
        public PedidosMappingProfile()
        {
            CreateMap<Pedido, PedidoDto>();

            // Mapeamento de Pedido para CarrinhoDto
            CreateMap<Pedido, CarrinhoDto>()
                .ForMember(dest => dest.PedidoId, opt => opt.MapFrom(src => src.Id)) // Mapeia o Id para PedidoId
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.PedidoItems.Sum(i => i.Quantidade * i.ValorUnitario))) // Calcula o SubTotal
                .ForMember(dest => dest.ValorTotal, opt => opt.MapFrom(src => src.ValorTotal)) // Mapeia o ValorTotal diretamente
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PedidoItems)); // Mapeia os itens do pedido

            // Mapeamento de PedidoItem para CarrinhoItemDto
            CreateMap<PedidoItem, CarrinhoItemDto>()
                .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProdutoId))
                .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.ProdutoNome))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.ValorUnitario, opt => opt.MapFrom(src => src.ValorUnitario))
                .ForMember(dest => dest.ValorTotal, opt => opt.MapFrom(src => src.CalcularValor())); // Utiliza o método CalcularValor para definir o ValorTotal
        }
    }
}
