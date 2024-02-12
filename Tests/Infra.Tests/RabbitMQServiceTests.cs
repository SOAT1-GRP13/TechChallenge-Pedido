using Domain.RabbitMQ;
using Infra.RabbitMQ;
using Moq;
using System.Text;
using RabbitMQ.Client;

namespace Infra.Tests
{
    public class RabbitMQServiceTests
    {
        [Fact]
        public void PublicaMensagem_ShouldPublishMessageToQueue()
        {
            // Configuração do Mock para IModel
            var mockModel = new Mock<IModel>();

            // Instanciação do serviço com a factory mockada
            var rabbitMQService = new RabbitMQService(mockModel.Object);

            // Definição dos parâmetros de teste
            var queueName = "testQueue";
            var message = "Test Message";
            var messageBody = Encoding.UTF8.GetBytes(message);

            // Execução do método a ser testado
            rabbitMQService.PublicaMensagem(queueName, message);

            mockModel.Verify(ch => ch.QueueDeclare(queueName, true, false, false, null), Times.Once);
        }

    }
}
