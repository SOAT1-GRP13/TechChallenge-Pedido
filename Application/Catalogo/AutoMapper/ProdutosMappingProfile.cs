using Application.Catalogo.Dto;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Catalogo.AutoMapper
{
    public class ProdutosMappingProfile : Profile
    {
        public ProdutosMappingProfile()
        {
            CreateMap<Produto, ProdutoDto>();

            CreateMap<ProdutoDto, Produto>()
                .ConstructUsing(p =>
                    new Produto(p.Nome, p.Descricao, p.Ativo,
                        p.Valor, p.CategoriaId, p.DataCadastro,
                        p.Imagem));
        }
    }
}