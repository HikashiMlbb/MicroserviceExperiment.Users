using Domain.ResetTokens;
using Domain.Users;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Persistence.ResetTokens;
using Testcontainers.Redis;

namespace PersistenceTests;

[TestFixture]
public class ResetTokenRepositoryTests
{
    private RedisContainer _container;
    private ResetTokenRepository _repo;
    private IDistributedCache _service;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _container = new RedisBuilder()
            .WithImage("redis:7.0")
            .Build();
        
        await _container.StartAsync();
    }

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = _container.GetConnectionString();
            options.InstanceName = "TestInstance";
        });
        var provider = services.BuildServiceProvider();
        _service = provider.GetRequiredService<IDistributedCache>();
        _repo = new ResetTokenRepository(_service);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
    

    [Test]
    public async Task Save_ReturnsSuccess()
    {
        // Arrange
        var email = new UserEmail("testemail@mail.com");
        var token = new ResetTokenValue(Guid.NewGuid().ToString());
        var expiration = ResetTokenExpiration.Create(DateTime.UtcNow + TimeSpan.FromHours(1)).Value!;
        var resetToken = new ResetToken(email, token, expiration);

        // Act
        await _repo.Save(resetToken);
        var foundEmailResult = await _service.GetStringAsync(token.Value);
        var isEmailRequested = await _service.GetStringAsync(email.Value);

        // Assert
        Assert.That(foundEmailResult, Is.Not.Null);
        Assert.That(isEmailRequested, Is.Not.Null);
        Assert.That(foundEmailResult, Is.EqualTo(email.Value));
        Assert.That(isEmailRequested, Is.Empty);
        
        // Clean
        await _service.RemoveAsync(token.Value);
        await _service.RemoveAsync(email.Value);
    }
    
    [Test]
    public async Task Find_ReturnsNull()
    {
        // Arrange
        var token = new ResetTokenValue(Guid.NewGuid().ToString());

        // Act
        var result = await _repo.Find(token);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task Find_ReturnsEmail()
    {
        // Arrange
        var email = new UserEmail("someemail@mail.com");
        var token = new ResetTokenValue(Guid.NewGuid().ToString());
        await _service.SetStringAsync(token.Value, email.Value);

        // Act
        var result = await _repo.Find(token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(email));
        
        // Clean
        await _service.RemoveAsync(token.Value);
    }

    [Test]
    public async Task IsRequested_NotExists_ReturnsFalse()
    {
        // Arrange
        var email = new UserEmail("some-notexisting-email@mail.com");

        // Act
        var isRequested = await _repo.IsRequested(email);
        
        // Assert
        Assert.That(isRequested, Is.False);
    }
    
    [Test]
    public async Task IsRequested_Exists_ReturnsTrue()
    {
        // Arrange
        var email = new UserEmail("some-notexisting-email@mail.com");
        await _service.SetStringAsync(email.Value, string.Empty);

        // Act
        var isRequested = await _repo.IsRequested(email);
        
        // Assert
        Assert.That(isRequested, Is.True);
        
        // Clean
        await _service.RemoveAsync(email.Value);
    }
}