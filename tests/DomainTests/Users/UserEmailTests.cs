using Domain.Users;

namespace DomainTests.Users;

[TestFixture]
public class UserEmailTests
{
    [Test]
    public void Create_ValidEmail_ReturnsSuccessResult()
    {
        // Arrange
        const string validEmail = "test@example.com";
        
        // Act
        var result = UserEmail.Create(validEmail);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(validEmail));
        });
    }

    [Test]
    public void Create_ValidEmailWithSubdomain_ReturnsSuccessResult()
    {
        // Arrange
        const string validEmail = "test.user+tag@sub.example.co.uk";
        
        // Act
        var result = UserEmail.Create(validEmail);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(validEmail));
        });
    }

    [Test]
    public void Create_EmailWithoutAtSymbol_ReturnsFailureResult()
    {
        // Arrange
        const string invalidEmail = "testexample.com";
        
        // Act
        var result = UserEmail.Create(invalidEmail);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_EmailWithoutDomain_ReturnsFailureResult()
    {
        // Arrange
        const string invalidEmail = "test@";
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_EmailWithInvalidCharacters_ReturnsFailureResult()
    {
        // Arrange
        const string invalidEmail = "te$t@example.com";
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_EmailWithSpace_ReturnsFailureResult()
    {
        // Arrange
        const string invalidEmail = "test user@example.com";
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_EmailWithMultipleDots_ReturnsFailureResult()
    {
        // Arrange
        const string invalidEmail = "test..user@example.com";
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_EmptyString_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = string.Empty;
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }

    [Test]
    public void Create_NullString_ReturnsFailureResult()
    {
        // Arrange
        string invalidEmail = null!;
        
        // Act
        var result = UserEmail.Create(invalidEmail);
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailIsInvalid));
        });
    }
}