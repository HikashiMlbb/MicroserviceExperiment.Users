using Infrastructure.ResetTokens;

namespace InfrastructureTests.ResetTokens;

using NUnit.Framework;
using System;
using System.Threading.Tasks;

[TestFixture]
public class ResetTokenServiceTests
{
    [Test]
    public async Task Generate_ReturnsValidToken()
    {
        // Arrange
        var settings = new ResetTokenSettings { Expiration = TimeSpan.FromHours(1) };
        var service = new ResetTokenService(settings);

        // Act
        var token = await service.Generate();

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token.Value, Is.Not.Empty);
    }

    [Test]
    public async Task GetExpiration_ReturnsValidExpiration()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expirationTimespan = TimeSpan.FromHours(1);
        var settings = new ResetTokenSettings { Expiration = expirationTimespan };
        var service = new ResetTokenService(settings);

        // Act
        var expiration = await service.GetExpiration();

        // Assert
        Assert.That(expiration.Value, Is.GreaterThan(now));
        Assert.That(expiration.Value, Is.EqualTo(now + expirationTimespan).Within(TimeSpan.FromMilliseconds(100)));
    }

    [Test]
    public void GetExpiration_ThrowsExceptionWhenExpirationSpanIsInvalid()
    {
        // Arrange
        var settings = new ResetTokenSettings { Expiration = TimeSpan.FromSeconds(-10) };
        var service = new ResetTokenService(settings);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => service.GetExpiration());
    }
}