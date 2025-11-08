namespace Infrastructure.ResetTokens;

public class RabbitMqSettings
{
    public string ConnectionString { get; init; } = null!;
    public string QueueName { get; set; }
}