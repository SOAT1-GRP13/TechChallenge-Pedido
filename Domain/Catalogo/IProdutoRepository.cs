namespace Domain.Catalogo
{
    public interface IProdutoRepository
    {
        Task<Produto> ObterPorId(Guid id);
    }
}