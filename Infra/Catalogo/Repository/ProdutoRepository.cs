using Domain.Base.Data;
using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace Infra.Catalogo.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly CatalogoContext _context;
        private readonly DbContextOptions<CatalogoContext> _optionsBuilder;
        protected readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public ProdutoRepository(CatalogoContext context, IConfiguration configuration, HttpClient httpClient, ILogger<ProdutoRepository> logger)
        {
            _optionsBuilder = new DbContextOptions<CatalogoContext>();
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            return await context.Produtos.AsNoTracking().ToListAsync();
        }

        public async Task<Produto> ObterPorId(Guid id)
        {
            var catalogoApiUrl = _configuration.GetSection("CatalogoApiUrl").Value;

            var response = await _httpClient.GetAsync($"{catalogoApiUrl}/Catalogo/busca_produto/{id}");

            if (response.IsSuccessStatusCode)
            {
                var produto = JsonConvert.DeserializeObject<Produto>(await response.Content.ReadAsStringAsync());
                return produto;
            }
            else
            {
                _logger.LogError(await response.Content.ReadAsStringAsync());
                return null;
            }
        }

        public async Task<IEnumerable<Produto>> ObterPorCategoria(int codigo)
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            return await context.Produtos.AsNoTracking().Include(p => p.Categoria).Where(c => c.Categoria.Codigo == codigo).ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> ObterCategorias()
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            return await context.Categorias.AsNoTracking().ToListAsync();
        }

        public async Task Adicionar(Produto produto)
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            produto.DataCadastro = DateTime.UtcNow;
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();
        }

        public async Task Atualizar(Produto produto)
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            context.Produtos.Update(produto);
            await context.SaveChangesAsync();
        }

        public async Task Remover(Produto produto)
        {
            using var context = new CatalogoContext(_optionsBuilder, _configuration);
            context.Produtos.Remove(produto);
            await context.SaveChangesAsync();
        }

        public void Adicionar(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
        }

        public void Atualizar(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _context?.Dispose();
        }
    }
}