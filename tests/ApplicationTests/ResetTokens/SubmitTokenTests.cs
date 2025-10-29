using Application.Abstractions;
using Application.ResetTokens;
using Application.ResetTokens.Submit;
using Application.Users;
using Domain.ResetTokens;
using Domain.Users;
using Moq;
using NUnit.Framework;

namespace ApplicationTests.ResetTokens;

[TestFixture]
public class SubmitTokenTests
{
    [Test]
    public async Task Handle_WhenTokenIsEmpty_ReturnsResetTokenErrorsEmpty()
    {
        // Arrange
        var tokenRepo = new Mock<IResetTokenRepository>();
        var passwordService = new Mock<IPasswordService>();
        var userRepo = new Mock<IUserRepository>();
        var handler = new SubmitResetTokenHandler(tokenRepo.Object, passwordService.Object, userRepo.Object);
        var submit = new SubmitResetToken { Token = null };

        // Act
        var result = await handler.Handle(submit);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(ResetTokenErrors.Empty));
    }

    [Test]
    public async Task Handle_WhenTokenNotFound_ReturnsResetTokenErrorsNotExistsOrExpired()
    {
        // Arrange
        var tokenRepo = new Mock<IResetTokenRepository>();
        tokenRepo.Setup(r => r.Find(It.IsAny<ResetTokenValue>()))
            .ReturnsAsync((UserEmail)null!);
        var passwordService = new Mock<IPasswordService>();
        var userRepo = new Mock<IUserRepository>();
        var handler = new SubmitResetTokenHandler(tokenRepo.Object, passwordService.Object, userRepo.Object);
        var submit = new SubmitResetToken { Token = "some-token" };

        // Act
        var result = await handler.Handle(submit);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(ResetTokenErrors.NotExistsOrExpired));
    }

    [Test]
    public async Task Handle_WhenNewPasswordIsNull_ReturnsSuccessWithNullResult()
    {
        // Arrange
        var email = UserEmail.Create("test-example@mail.com");
        var tokenRepo = new Mock<IResetTokenRepository>();
        tokenRepo.Setup(r => r.Find(It.IsAny<ResetTokenValue>())).ReturnsAsync(email.Value);
        var passwordService = new Mock<IPasswordService>();
        var userRepo = new Mock<IUserRepository>();
        var handler = new SubmitResetTokenHandler(tokenRepo.Object, passwordService.Object, userRepo.Object);
        var submit = new SubmitResetToken { Token = "some-token", NewPassword = null, ConfirmPassword = null };

        // Act
        var result = await handler.Handle(submit);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public async Task Handle_WhenNewPasswordAndConfirmPasswordDontMatch_ReturnsUserApplicationErrorsLoginFailed()
    {
        // Arrange
        var email = UserEmail.Create("test-example@mail.com");
        var tokenRepo = new Mock<IResetTokenRepository>();
        tokenRepo.Setup(r => r.Find(It.IsAny<ResetTokenValue>())).ReturnsAsync(email.Value);
        var passwordService = new Mock<IPasswordService>();
        var userRepo = new Mock<IUserRepository>();
        var handler = new SubmitResetTokenHandler(tokenRepo.Object, passwordService.Object, userRepo.Object);
        var submit = new SubmitResetToken { Token = "some-token", NewPassword = "new-password", ConfirmPassword = "other-password" };

        // Act
        var result = await handler.Handle(submit);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(UserApplicationErrors.LoginFailed));
    }

    [Test]
    public async Task Handle_WhenNewPasswordIsValid_ChangesPasswordAndReturnsSuccess()
    {
        // Arrange
        var email = UserEmail.Create("test-example@mail.com");
        var hashedPassword = new UserPassword("h@5h3d-p@55w0rd");
        var tokenRepo = new Mock<IResetTokenRepository>();
        tokenRepo.Setup(r => r.Find(It.IsAny<ResetTokenValue>())).ReturnsAsync(email.Value);
        var passwordService = new Mock<IPasswordService>();
        passwordService.Setup(s => s.Hash(It.IsAny<UserRawPassword>())).ReturnsAsync(hashedPassword);
        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.ChangePassword(email.Value!, hashedPassword)).Returns(Task.CompletedTask);
        var handler = new SubmitResetTokenHandler(tokenRepo.Object, passwordService.Object, userRepo.Object);
        var submit = new SubmitResetToken { Token = "some-token", NewPassword = "new-password", ConfirmPassword = "new-password" };

        // Act
        var result = await handler.Handle(submit);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(string.Empty));
        userRepo.Verify(r => r.ChangePassword(email.Value!, hashedPassword), Times.Once);
    }
}