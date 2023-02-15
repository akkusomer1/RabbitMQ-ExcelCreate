using System.Text;
using System.Text.Json;
using SharedLibrary;

namespace RabbitMQWeb.ExcelCreate.Service
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _rabbitMqClientService.Connect();

            string bodyJsonString = JsonSerializer.Serialize(createExcelMessage);

            var bodyByte = Encoding.UTF8.GetBytes(bodyJsonString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: RabbitMqClientService.ExchangeName,
                routingKey: RabbitMqClientService.RoutingExcel,
                basicProperties: properties,
                body: bodyByte,
                mandatory:default);
        }
    }
}
