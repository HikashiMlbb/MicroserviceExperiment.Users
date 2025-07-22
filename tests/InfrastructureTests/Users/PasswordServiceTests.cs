using Domain.Users;
using Infrastructure.Services;

namespace InfrastructureTests.Users;

[TestFixture]
public class PasswordServiceTests
{
    private PasswordService _service;
    
    [OneTimeSetUp]
    public void Setup()
    {
        _service = new PasswordService();
    }

    [Test]
    public async Task HashPassword_NotNull()
    {
        // Arrange
        var password = new UserRawPassword("SomePassword");
        
        // Act
        var result = await _service.Hash(password);
        
        // Assert
        Assert.That(result.Value, Is.Not.Null);
    }
    
    [Test]
    public async Task VerifySamePasswords_ReturnsSuccess()
    {
        // Arrange
        const string password = "SomePassword";
        var rawPassword = new UserRawPassword(password);
        
        // Act
        var hashedPassword = await _service.Hash(rawPassword);
        var result = await _service.Verify(rawPassword, hashedPassword);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public async Task VerifyDifferentPasswords_ReturnsFailure()
    {
        // Arrange
        const string password = "SomePassword";
        var rawPassword = new UserRawPassword(password);
        
        // Act
        var hashedPassword = await _service.Hash(rawPassword);
        var result = await _service.Verify(new UserRawPassword("23jhfdghdfguidfuhg"), hashedPassword);
        
        // Assert
        Assert.That(result, Is.False);
    }
}