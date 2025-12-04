using Infrastructure.ResetTokens;
using Infrastructure.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace API.Tests.Abstractions;

[TestFixture]
public abstract class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected PostgreSqlContainer Postgres;
    protected RedisContainer Redis;
    protected RabbitMqContainer Rabbit;
    protected HttpClient Http;

    [OneTimeSetUp]
    public async Task SetUpOnce()
    {
        Postgres = new PostgreSqlBuilder().Build();
        Redis = new RedisBuilder().Build();
        Rabbit = new RabbitMqBuilder().Build();
        await Postgres.StartAsync();
        await Redis.StartAsync();
        await Rabbit.StartAsync();
        Http = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("CONNECTION_STRING", Postgres.GetConnectionString());
        builder.UseSetting("CACHE_CONNECTION", Redis.GetConnectionString());
        var rabbitMqSettings = new RabbitMqSettings { ConnectionString = Rabbit.GetConnectionString(), QueueName = "TestQueue" };
        var authorizationTokenSettings = new AuthorizationTokenConfig
        {
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            Expiration = TimeSpan.FromHours(1),
            Key = Guid.NewGuid().ToString() + Guid.NewGuid()
        };
        var resetTokenSettings = new ResetTokenSettings
        {
            Url = "http://localhost:5156/",
            Expiration = TimeSpan.FromMinutes(10)
        };

        builder.ConfigureServices(x =>
        {
            x.RemoveAll(typeof(RabbitMqSettings));
            x.AddSingleton(rabbitMqSettings);

            x.RemoveAll(typeof(IOptions<AuthorizationTokenConfig>));
            x.AddSingleton(Options.Create(authorizationTokenSettings));

            x.RemoveAll(typeof(IOptions<ResetTokenSettings>));
            x.AddSingleton(Options.Create(resetTokenSettings));
        });
    }

    [OneTimeTearDown]
    public async Task TearDownOnce()
    {
        await Postgres.StopAsync();
        await Redis.StopAsync();
        await Rabbit.StopAsync();
    }
}