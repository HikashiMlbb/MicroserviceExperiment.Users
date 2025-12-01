namespace Infrastructure.ResetTokens;

public class RabbitMqSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}