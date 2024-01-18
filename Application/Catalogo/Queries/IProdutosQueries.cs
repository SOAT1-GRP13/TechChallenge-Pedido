using Application.Catalogo.Dto;

namespace Application.Catalogo.Queries
{
    public interface IProdutosQueries : IDisposable
    {
        Task<ProdutoDto> ObterPorId(Guid id);
    }
}
