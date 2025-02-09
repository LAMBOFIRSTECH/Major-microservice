using System.Text;
using TasksManagement_API.Interfaces;
using RabbitMQ.Client;
namespace TasksManagement_API.Services;
public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> logger;
    private readonly IHashicorpVaultService hashicorpVaultService;
    public RabbitMqService(ILogger<RabbitMqService> logger, IHashicorpVaultService hashicorpVaultService)
    {
        this.logger = logger;
        this.hashicorpVaultService = hashicorpVaultService;
    }
    private async Task<ConnectionFactory> EstablishConnection()
    {
        var connectionString = await hashicorpVaultService.GetRabbitConnectionStringFromVault();
        var rabbitUri = new Uri("amqp://" + connectionString);
        return new ConnectionFactory { Uri = rabbitUri };
    }
    public async void SendToRabbitMq(string message)
    {
        var factory = await EstablishConnection();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "authentification",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "",
                             routingKey: "authentification",
                             basicProperties: null,
                             body: body);
        logger.LogInformation(" [x] Sent {message}", message);
        connection.Close();
    }
}