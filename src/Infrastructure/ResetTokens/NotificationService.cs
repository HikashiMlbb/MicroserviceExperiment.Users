using System.Text;
using System.Text.Json;
using Application.Abstractions;
using Domain.ResetTokens;
using RabbitMQ.Client;

namespace Infrastructure.ResetTokens;

public class NotificationService : INotificationService, IAsyncDisposable
{
    private readonly string _queueName;
    private readonly string _tokenUrl;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private bool _isDisposed;

    public NotificationService(RabbitMqSettings settings, ResetTokenSettings tokenSettings)
    {
        _queueName = settings.QueueName;
        _tokenUrl = tokenSettings.Url;
        
        var factory = new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionString)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(_queueName, true, false, false).GetAwaiter().GetResult();
    }
    
    public async Task Notify(ResetToken token)
    {
        var body = $"<h1>We're detected you're trying to reset password. If it was you, please, follow the link below:</h1><p><a href=\"{_tokenUrl}/{token.Token.Value}\"></p><p>But if it wasn't you, please, ignore this email.</p><p>Best wishes, Hikashi no Development!</p>";
        var message = new Message("Password Reset", body, token.Email.Value);
        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        await _channel.BasicPublishAsync(string.Empty, _queueName, body: messageBytes);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        
        await _channel.CloseAsync();
        await _channel.DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}