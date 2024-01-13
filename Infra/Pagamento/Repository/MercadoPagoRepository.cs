using System.Text;
using System.Text.Json;
using Domain.Configuration;
using Domain.Pagamento;
using Domain.Pedidos;
using Microsoft.Extensions.Options;

namespace Infra.Pagamento.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly Secrets _settings;

        public PagamentoRepository(IOptions<Secrets> options)
        {
            _settings = options.Value;
        }

        public async Task<string> GeraPedidoQrCode(Pedido pedido)
        {
            var itensList = new List<OrderItem>();

            foreach (var orderItem in pedido.PedidoItems.ToList())
            {
                itensList.Add(new OrderItem(orderItem));
            }


            var dto = new MercadoPagoOrder(pedido, itensList);

            var client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.PagamentoApiUrl)
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/pagamento/MercadoPago/GerarQR");

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var order = JsonSerializer.Serialize(dto, serializeOptions);
            var content = new StringContent(order, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var qrResponse = JsonSerializer.Deserialize<GerarQRResponse>(await response.Content.ReadAsStringAsync(), serializeOptions);
                return qrResponse!.QRData;
            }
            return string.Empty;
        }
    }

}