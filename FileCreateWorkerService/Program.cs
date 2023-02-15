using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext,services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.AddDbContext<AdventureWorks2019Context>(opt =>
                    {
                        opt.UseSqlServer(configuration.GetConnectionString("SqlServer2"));
                    });

                    var rabbitMqUri = configuration.GetConnectionString("RabbitMqUri");
                    services.AddSingleton(sp =>
                    {
                        return new ConnectionFactory { Uri = new Uri($"{rabbitMqUri}") };
                    });

                    services.AddSingleton<RabbitMqClientService>();
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}