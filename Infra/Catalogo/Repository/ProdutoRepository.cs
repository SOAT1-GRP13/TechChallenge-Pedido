using Domain.Base.Data;
using Domain.Catalogo;
using Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;

namespace Infra.Catalogo.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        protected readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly Secrets _secrets;

        public ProdutoRepository(IConfiguration configuration, 
        HttpClient httpClient, ILogger<ProdutoRepository> logger, IOptions<Secrets> settings)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
            _secrets = settings.Value;
            _httpClient.BaseAddress = new Uri(_secrets.CatalogoApiUrl);
        }

        public async Task<Produto> ObterPorId(Guid id)
        {
            var response = await _httpClient.GetAsync($"/produto/Catalogo/busca_produto/{id}");

            if (response.IsSuccessStatusCode)
            {
                var produto = JsonConvert.DeserializeObject<Produto>(await response.Content.ReadAsStringAsync());
                return produto!;
            }
            else
            {
                _logger.LogInformation(await response.Content.ReadAsStringAsync());
                return null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}