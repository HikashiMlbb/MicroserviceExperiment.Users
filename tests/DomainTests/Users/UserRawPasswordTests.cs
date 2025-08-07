using Domain.Users;

namespace DomainTests.Users;

using NUnit.Framework;

[TestFixture]
public class UserRawPasswordTests
{
    [Test]
    public void Create_ValidPassword_ReturnsSuccessResult()
    {
        // Arrange
        const string validPassword = "ValidPass123";
        
        // Act
        var result = UserRawPassword.Create(validPassword);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(validPassword));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Create_NullOrWhiteSpace_ReturnsFailureResult(string? invalidPassword)
    {
        // Act
        var result = UserRawPassword.Create(invalidPassword!);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserDomainErrors.PasswordIsOutOfRange));
        });
    }

    [TestCase("1234")]
    [TestCase("123")]
    [TestCase("12345678901234567890")] 
    [TestCase("123456789012345678901")]
    public void Create_OutOfRangeLength_ReturnsFailureResult(string invalidPassword)
    {
        // Act
        var result = UserRawPassword.Create(invalidPassword);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserDomainErrors.PasswordIsOutOfRange));
        });
    }

    [TestCase("12345")]
    [TestCase("1234567890123456789")]
    [TestCase("GoodPassword1")]
    public void Create_BorderlineLengths_ReturnsSuccessResult(string validPassword)
    {
        // Act
        var result = UserRawPassword.Create(validPassword);
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Value, Is.EqualTo(validPassword));
        });
    }
}