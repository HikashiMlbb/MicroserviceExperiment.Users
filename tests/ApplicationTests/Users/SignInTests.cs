using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Domain.Users;
using Moq;
using NUnit.Framework;

namespace ApplicationTests.Users;

[TestFixture]
public class SignInTests
{
    private Mock<IPasswordService> _passwordMock;
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IAuthorizationTokenService> _tokenMock;
    private UserSignInHandler _handler;

    [SetUp]
    public void Setup()
    {
        _passwordMock = new Mock<IPasswordService>();
        _uowMock = new Mock<IUnitOfWork>();
        _tokenMock = new Mock<IAuthorizationTokenService>();
        _handler = new UserSignInHandler(_uowMock.Object, _passwordMock.Object, _tokenMock.Object);
    }

    [Test]
    public async Task NotFound_ReturnsError()
    {
        // Arrange
        const string username = "SomeUsername";
        const string password = "SomePassword";
        var dto = new UserSignIn
        {
            Username = username,
            Password = password
        };
        _uowMock.Setup(x => x.Users.Fetch(It.IsAny<UserName>())).ReturnsAsync((User)null!);

        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserApplicationErrors.NotFound));
        _uowMock.Verify(x => x.Users.Fetch(It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>()), Times.Never);
    }
    
    [Test]
    public async Task PasswordIsIncorrect_ReturnsError()
    {
        // Arrange
        const string username = "SomeUsername";
        const string password = "SomePassword";
        var dto = new UserSignIn
        {
            Username = username,
            Password = password
        };
        var user = new User(new UserId(15), new UserEmail("someemail@mail.com"), new UserName(username), new UserPassword(password));
        _uowMock.Setup(x => x.Users.Fetch(It.IsAny<UserName>())).ReturnsAsync(user);
        _passwordMock.Setup(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>())).ReturnsAsync(false);
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserApplicationErrors.LoginFailed));
        _uowMock.Verify(x => x.Users.Fetch(It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>()), Times.Once);
        _tokenMock.Verify(x => x.GenerateToken(It.IsAny<AuthorizationTokenPayload>()), Times.Never);
    }
    
    [Test]
    public async Task CreateSuccessfully_ReturnsSuccess()
    {
        // Arrange
        const string username = "SomeUsername";
        const string password = "SomePassword";
        var dto = new UserSignIn
        {
            Username = username,
            Password = password
        };
        var userId = new UserId(15);
        var expectedToken = $"Token #{userId}$$$";
        var user = new User(userId, new UserEmail("someemail@mail.com"), new UserName(username), new UserPassword(password));
        _uowMock.Setup(x => x.Users.Fetch(It.IsAny<UserName>())).ReturnsAsync(user);
        _passwordMock.Setup(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>())).ReturnsAsync(true);
        _tokenMock.Setup(x => x.GenerateToken(It.IsAny<AuthorizationTokenPayload>())).ReturnsAsync(expectedToken);
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!, Is.EqualTo(expectedToken));
        _uowMock.Verify(x => x.Users.Fetch(It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>()), Times.Once);
        _tokenMock.Verify(x => x.GenerateToken(It.IsAny<AuthorizationTokenPayload>()), Times.Once);
    }
}