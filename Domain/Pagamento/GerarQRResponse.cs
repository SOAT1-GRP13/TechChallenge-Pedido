using System.Text.Json.Serialization;

namespace Domain.Pagamento
{
    public class GerarQRResponse {

        public GerarQRResponse()
        {
            QRData = string.Empty;
        }

        [JsonPropertyName("qr_data")]
        public string QRData {get;set;}
    }
}