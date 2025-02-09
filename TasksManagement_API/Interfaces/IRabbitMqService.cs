namespace TasksManagement_API.Interfaces;
public interface IRabbitMqService
{
    void SendToRabbitMq(string message);
}