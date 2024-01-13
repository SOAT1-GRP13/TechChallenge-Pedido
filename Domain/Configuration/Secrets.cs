namespace Domain.Configuration
{
    public class Secrets
    {
        public Secrets()
        {
            ClientSecret = string.Empty;
            PreSalt = string.Empty;
            PosSalt = string.Empty;
            ConnectionString = string.Empty;
            CatalogoApiUrl = string.Empty;
            PagamentoApiUrl = string.Empty;
        }

        public string ClientSecret { get; set; }
        public string PreSalt { get; set; }
        public string PosSalt { get; set; }
        public string ConnectionString { get; set; }
        public string CatalogoApiUrl { get; set; }
        public string PagamentoApiUrl { get; set; }
    }
}