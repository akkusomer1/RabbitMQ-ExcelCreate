using System.Data;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLibrary;

namespace FileCreateWorkerService
{

    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqClientService _rabbitMqClientService;
        private IModel _channel;
        public Worker(ILogger<Worker> logger, RabbitMqClientService rabbitMqClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMqClientService = rabbitMqClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMqClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }

        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMqClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received1;

            return  Task.CompletedTask;
        }

        private void Consumer_Received1(object? sender, BasicDeliverEventArgs e)
        {
            Task.Delay(5000).Wait();
            _logger.LogInformation("Consumer_Received girildi");
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(message);

            using var memoryStream = new MemoryStream();

            var workBook = new XLWorkbook();
            DataSet dataSet = new DataSet();

            dataSet.Tables.Add(GetTable("products"));

            workBook.Worksheets.Add(dataSet);

            workBook.SaveAs(memoryStream);

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();

            var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());

            multipartFormDataContent.Add(byteArrayContent, "file", Guid.NewGuid().ToString() + ".xlsx");

            string baseUrl = "https://localhost:7106/api/files";

            using (var httpClient = new HttpClient())
            {
                var response =  httpClient
                    .PostAsync($"{baseUrl}?fileId={createExcelMessage.FileId}", multipartFormDataContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{createExcelMessage.FileId} numaralý tablo baþarýlý bir þekilde oluþturuldu");
                    _channel.BasicAck(e.DeliveryTag, false);
                }
            }
        }

      

       


        //var messageCount = _channel.MessageCount(RabbitMqClientService.QueueName);

     
        private DataTable GetTable(string tableName)
        {
            List<Product> products;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdventureWorks2019Context>();
                 products = context.Set<Product>().ToList();
            }

            DataTable dataTable = new DataTable()
            {
                TableName = tableName,
                Columns =
                {
                    new DataColumn("ProductId",typeof(int)),
                    new DataColumn("Name",typeof(string)),
                    new DataColumn("ProductNumber",typeof(string)),
                    new DataColumn("Color",typeof(string)),
                }
            };

            products.ToList().ForEach(x =>
            {
                dataTable.Rows.Add(x.ProductId, x.Name, x.ProductNumber, x.Color);
            });
            return dataTable;
        }
    }
}
