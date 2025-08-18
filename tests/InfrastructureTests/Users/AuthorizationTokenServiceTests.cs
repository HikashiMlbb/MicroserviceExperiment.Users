using Application.Abstractions;
using Domain.Users;
using Infrastructure.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace InfrastructureTests.Users;

[TestFixture]
public class AuthorizationTokenServiceTests
{
    private AuthorizationTokenService _tokenService;
    private AuthorizationTokenDescription _tokenDescription;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var config = new AuthorizationTokenConfig
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Expiration = TimeSpan.FromHours(1),
            Key = "test-key-1234567890-1234567890-1234567890-1234567890"
        };
        
        var options = Options.Create(config);
        _tokenDescription = new AuthorizationTokenDescription(options);
        _tokenService = new AuthorizationTokenService(_tokenDescription);
    }

    [Test]
    public async Task GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));
        var handler = new JsonWebTokenHandler();

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var isValid = await handler.ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(isValid.Exception, Is.Null);
    }
    
    [Test]
    public async Task GenerateInvalidToken_ShouldReturnException()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));
        var handler = new JsonWebTokenHandler();

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var isValid = await handler.ValidateTokenAsync(token + "1234", GetValidationParameters());
        
        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(isValid.Exception, Is.Not.Null);
    }

    [Test]
    public async Task GenerateToken_ShouldContainCorrectSubjectClaim()
    {
        // Arrange
        const int expectedId = 15;
        var payload = new AuthorizationTokenPayload(new UserId(expectedId));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        var subClaim = jwtToken.Claims.FirstOrDefault(c =>  c.Key == JwtRegisteredClaimNames.Sub);
        Assert.That(subClaim.Value, Is.EqualTo(expectedId.ToString()));
    }
    
    [Test]
    public async Task GenerateInvalidToken_ShouldContainIncorrectSubjectClaim()
    {
        // Arrange
        const int expectedId = 15;
        var payload = new AuthorizationTokenPayload(new UserId(expectedId + 18));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        var subClaim = jwtToken.Claims.FirstOrDefault(c =>  c.Key == JwtRegisteredClaimNames.Sub);
        Assert.That(subClaim.Value, Is.Not.EqualTo(expectedId.ToString()));
    }

    [Test]
    public async Task GenerateToken_ShouldContainCorrectIssuerAndAudience()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.Issuer, Is.EqualTo(_tokenDescription.Issuer));
        Assert.That(jwtToken.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Aud)?.Value, Is.EqualTo(_tokenDescription.Audience));
    }
    
    [Test]
    public async Task GenerateToken_ShouldContainIncorrectIssuerAndCorrectAudience()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.Issuer, Is.Not.EqualTo("some jwtToken.Issuer"));
        Assert.That(jwtToken.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Aud)?.Value, Is.EqualTo(_tokenDescription.Audience));
    }
    
    [Test]
    public async Task GenerateToken_ShouldContainCorrectIssuerAndIncorrectAudience()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.Issuer, Is.EqualTo(_tokenDescription.Issuer));
        Assert.That(jwtToken.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Aud)?.Value, Is.Not.EqualTo("some _tokenDescription.Audience"));
    }
    
    [Test]
    public async Task GenerateToken_ShouldContainIncorrectIssuerAndAudience()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));

        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.Issuer, Is.Not.EqualTo("some _tokenDescription.Issuer"));
        Assert.That(jwtToken.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Aud)?.Value, Is.Not.EqualTo("some _tokenDescription.Audience"));
    }

    [Test]
    public async Task GenerateToken_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));
        var expectedExpiration = DateTime.UtcNow.Add(_tokenDescription.Expiration);
        
        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.SecurityToken.ValidTo, Is.EqualTo(expectedExpiration).Within(5).Seconds);
    }
    
    [Test]
    public async Task GenerateToken_ShouldHaveIncorrectExpiration()
    {
        // Arrange
        var payload = new AuthorizationTokenPayload(new UserId(15));
        var expectedExpiration = DateTime.UtcNow.Add(_tokenDescription.Expiration);
        
        // Act
        var token = await _tokenService.GenerateToken(payload);
        var jwtToken = await new JsonWebTokenHandler().ValidateTokenAsync(token, GetValidationParameters());
        
        // Assert
        Assert.That(jwtToken.SecurityToken.ValidTo + TimeSpan.FromHours(13), Is.Not.EqualTo(expectedExpiration).Within(5).Seconds);
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _tokenDescription.Issuer,
            ValidAudience = _tokenDescription.Audience,
            IssuerSigningKey = _tokenDescription.Key,
            ClockSkew = TimeSpan.Zero
        };
    }
}