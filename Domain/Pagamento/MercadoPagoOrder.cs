using System.Globalization;
using Domain.Pedidos;

namespace Domain.Pagamento
{
    public class MercadoPagoOrder
    {
        public MercadoPagoOrder()
        {
            External_reference = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            Expiration_date = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
            Total_amount = 0;
            Items = new List<OrderItem>();
        }

        public MercadoPagoOrder(Pedido pedido, List<OrderItem> orderItems)
        {
            External_reference = pedido.Id.ToString();
            Title = "Pedido confirmado"; //TODO preencher com titulo do pedido
            Description = "Descrição do pedido";
            Expiration_date = DateTime.Now.AddMinutes(20).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");
            Total_amount = pedido.ValorTotal;
            Items = orderItems;
        }

        public string External_reference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Expiration_date { get; set; }
        public decimal Total_amount { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}