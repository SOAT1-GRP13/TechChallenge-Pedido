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
            Rabbit_Hostname = string.Empty;
            Rabbit_Password = string.Empty;
            Rabbit_Username = string.Empty;
            Rabbit_Port = string.Empty;
            ExchangePedidoConfirmado = string.Empty;
            ExchangePedidoRecusado = string.Empty;
            ExchangePedidoPago = string.Empty;
            ExchangePedidoPreparando = string.Empty;
            ExchangePedidoPronto = string.Empty;
            ExchangePedidoFinalizado = string.Empty;
            QueuePedidoRecusado = string.Empty;
            QueuePedidoPago = string.Empty;
            QueuePedidoPreparando = string.Empty;
            QueuePedidoPronto = string.Empty;
            QueuePedidoFinalizado = string.Empty;
            Rabbit_VirtualHost = string.Empty;
        }

        public string ClientSecret { get; set; }
        public string PreSalt { get; set; }
        public string PosSalt { get; set; }
        public string ConnectionString { get; set; }
        public string CatalogoApiUrl { get; set; }
        public string PagamentoApiUrl { get; set; }

        public string Rabbit_Hostname { get; set; }
        public string Rabbit_Port { get; set; }
        public string Rabbit_Username { get; set; }
        public string Rabbit_Password { get; set; }
        public string ExchangePedidoConfirmado { get; set; }
        public string ExchangePedidoRecusado { get; set; }
        public string ExchangePedidoPago { get; set; }
        public string ExchangePedidoPreparando { get; set; }
        public string ExchangePedidoPronto { get; set; }
        public string ExchangePedidoFinalizado { get; set; }
        public string QueuePedidoRecusado { get; set; }
        public string QueuePedidoPago { get; set; }
        public string QueuePedidoPreparando { get; set; }
        public string QueuePedidoPronto { get; set; }
        public string QueuePedidoFinalizado { get; set; }
        public string Rabbit_VirtualHost { get; set; }
    }
}