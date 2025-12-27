using System.Text;
using System.Text.Json;
using Domain.ResetTokens;
using Domain.Users;
using Infrastructure.ResetTokens;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Testcontainers.RabbitMq;

namespace InfrastructureTests.ResetTokens;

[TestFixture]
public class NotificationServiceTests
{
    private RabbitMqContainer _container = null!;
    
    [OneTimeSetUp]
    public async Task SetupOnce()
    {
        _container = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.11")
            .Build();
        await _container.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDownOnce()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    [Test]
    public void EmptyTest()
    {
        Assert.Pass();
    }

    [Test]
    public async Task Notify_Successfully()
    {
        // Arrange
        var factory = new ConnectionFactory { Uri = new Uri(_container.GetConnectionString()) };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        const string html = "<h1>We're detected you're trying to reset password. If it was you, please, follow the link below:</h1><p><a href=\"${{ token }}\"></p><p>But if it wasn't you, please, ignore this email.</p><p>Best wishes, Hikashi no Development!</p>";
        var template = new EmailTemplate { Template = html };
        var templateService = new EmailTemplateService(template);
        
        const string queueName = "test-queue-name";
        var rabbitMqSettings = new RabbitMqSettings { ConnectionString = _container.GetConnectionString(), QueueName = queueName };
        var resetTokenSettings = new ResetTokenSettings { Expiration = TimeSpan.FromHours(2), Url = "http://test-url" };
        await using var service = new NotificationService(rabbitMqSettings, resetTokenSettings, templateService);

        var tokenValue = Guid.NewGuid().ToString();
        var token = new ResetToken(UserEmail.Create("some-email@mail.com").Value!, new ResetTokenValue(tokenValue), ResetTokenExpiration.Create(DateTime.UtcNow + TimeSpan.FromHours(2)).Value!);

        // Act
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, @event) =>
        {
            var body = Encoding.UTF8.GetString(@event.Body.ToArray());
            var result = JsonSerializer.Deserialize<Message>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            await TestContext.Out.WriteLineAsync($"{result!.Topic} -- {result.Body} -- {result.Recipient}");
            tcs.SetResult(true);
        };
        await channel.BasicConsumeAsync(queueName, true, consumer);
        await service.Notify(token);

        var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // Assert
        Assert.That(result, Is.EqualTo(true));
    }
}