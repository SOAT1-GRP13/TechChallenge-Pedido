using Application.Catalogo.Dto;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Catalogo.Queries
{
    public class ProdutosQueries : IProdutosQueries
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMapper _mapper;

        public ProdutosQueries(IProdutoRepository produtoRepository,
                                IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _mapper = mapper;
        }

        public async Task<ProdutoDto> ObterPorId(Guid id)
        {
            return _mapper.Map<ProdutoDto>(await _produtoRepository.ObterPorId(id));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
