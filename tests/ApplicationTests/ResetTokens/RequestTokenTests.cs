using Application.Abstractions;
using Application.ResetTokens;
using Application.ResetTokens.Request;
using Application.Users;
using Domain.ResetTokens;
using Domain.Users;
using Moq;
using NUnit.Framework;

namespace ApplicationTests.ResetTokens;

public class RequestTokenTests
{
    private RequestResetTokenHandler _handler;
    private Mock<IUnitOfWork> _uow;
    private Mock<IResetTokenService> _tokenServiceMock;
    private Mock<INotificationService> _notificationServiceMock;

    [SetUp]
    public void Setup()
    {
        _uow = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<IResetTokenService>();
        _notificationServiceMock = new Mock<INotificationService>();

        _handler = new RequestResetTokenHandler(
            _uow.Object,
            _tokenServiceMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_WithInvalidEmail_ReturnsValidationError()
    {
        // Arrange
        var request = new RequestResetToken { Email = "invalid_email" };

        // Act
        var result = await _handler.Handle(request);

        // Assert
        Assert.That(result.Error, Is.EqualTo(ResetTokenErrors.ValidationError));
        _uow.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Never);
        _uow.Verify(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>()), Times.Never);
        _tokenServiceMock.Verify(x => x.Generate(), Times.Never);
        _tokenServiceMock.Verify(x => x.GetExpiration(), Times.Never);
        _uow.Verify(x => x.ResetTokens.Save(It.IsAny<ResetToken>()), Times.Never);
        _notificationServiceMock.Verify(x => x.Notify(It.IsAny<ResetToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithNonExistingUser_ReturnsNotFoundError()
    {
        // Arrange
        var request = new RequestResetToken { Email = "test@example.com" };
        _uow.Setup(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>())).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        Assert.That(result.Error, Is.EqualTo(UserApplicationErrors.NotFound));
        _uow.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Once);
        _uow.Verify(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>()), Times.Never);
        _tokenServiceMock.Verify(x => x.Generate(), Times.Never);
        _tokenServiceMock.Verify(x => x.GetExpiration(), Times.Never);
        _uow.Verify(x => x.ResetTokens.Save(It.IsAny<ResetToken>()), Times.Never);
        _notificationServiceMock.Verify(x => x.Notify(It.IsAny<ResetToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithAlreadyRequestedToken_ReturnsAlreadyRequestedError()
    {
        // Arrange
        var request = new RequestResetToken { Email = "test@example.com" };
        _uow.Setup(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>())).ReturnsAsync(true);
        _uow.Setup(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>())).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        Assert.That(result.Error, Is.EqualTo(ResetTokenErrors.AlreadyRequested));
        
        _uow.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Once);
        _uow.Verify(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>()), Times.Once);
        _tokenServiceMock.Verify(x => x.Generate(), Times.Never);
        _tokenServiceMock.Verify(x => x.GetExpiration(), Times.Never);
        _uow.Verify(x => x.ResetTokens.Save(It.IsAny<ResetToken>()), Times.Never);
        _notificationServiceMock.Verify(x => x.Notify(It.IsAny<ResetToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new RequestResetToken { Email = "test@example.com" };
        _uow.Setup(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>())).ReturnsAsync(true);
        _uow.Setup(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>())).ReturnsAsync(false);
        _tokenServiceMock.Setup(x => x.Generate()).ReturnsAsync(new ResetTokenValue("f923929jjgfddg"));
        _tokenServiceMock.Setup(x => x.GetExpiration()).ReturnsAsync(ResetTokenExpiration.Create(System.DateTime.UtcNow.AddHours(1)).Value!);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _uow.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Once);
        _uow.Verify(x => x.ResetTokens.IsRequested(It.IsAny<UserEmail>()), Times.Once);
        _tokenServiceMock.Verify(x => x.Generate(), Times.Once);
        _tokenServiceMock.Verify(x => x.GetExpiration(), Times.Once);
        _uow.Verify(x => x.ResetTokens.Save(It.IsAny<ResetToken>()), Times.Once);
        _notificationServiceMock.Verify(x => x.Notify(It.IsAny<ResetToken>()), Times.Once);
    }
}