using Domain.Pagamento;
using System.Text.Json;

namespace Domain.Tests.Pagamento
{
    public class GerarQRResponseTests
    {
        [Fact]
        public void GerarQRResponse_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var qrResponse = new GerarQRResponse { QRData = "ExampleQRData" };
            var expectedJson = "{\"qr_data\":\"ExampleQRData\"}";

            // Act
            var serializedJson = JsonSerializer.Serialize(qrResponse);
            var deserializedObject = JsonSerializer.Deserialize<GerarQRResponse>(expectedJson);

            // Assert
            Assert.Equal(expectedJson, serializedJson);
            Assert.NotNull(deserializedObject);
            Assert.Equal(qrResponse.QRData, deserializedObject.QRData);
        }
    }
}
