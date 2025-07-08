using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Domain.Users;
using Moq;
using NUnit.Framework;
using UserDomainErrors = Domain.Users.UserErrors;
using UserErrors = Application.Users.UserErrors;

namespace ApplicationTests.Users;

[TestFixture]
public class SignInTests
{
    private Mock<IPasswordService> _passwordMock;
    private Mock<IUserRepository> _repoMock;
    private Mock<IAuthorizationTokenService> _tokenMock;
    private UserSignInHandler _handler;

    [SetUp]
    public void Setup()
    {
        _passwordMock = new Mock<IPasswordService>();
        _repoMock = new Mock<IUserRepository>();
        _tokenMock = new Mock<IAuthorizationTokenService>();
        _handler = new UserSignInHandler(_passwordMock.Object, _repoMock.Object, _tokenMock.Object);
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
        _repoMock.Setup(x => x.Fetch(It.IsAny<UserName>())).ReturnsAsync((User)null!);

        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserErrors.NotFound));
        _repoMock.Verify(x => x.Fetch(It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Verify(It.IsAny<UserRawPassword>(), It.IsAny<UserPassword>()), Times.Never);
    }
}