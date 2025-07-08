using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Domain.Users;
using Moq;
using NUnit.Framework;
using UserDomainErrors = Domain.Users.UserErrors;

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
    public async Task InvalidUsername_ReturnsError()
    {
        // Arrange
        const string username = "Some Very Strange Username$";
        const string password = "SomeSecretPassword";
        var dto = new UserSignIn
        {
            Username = username,
            Password = password
        };

        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserDomainErrors.UsernameIsInvalid));
        _repoMock.Verify(x => x.Fetch(It.IsAny<UserName>()), Times.Never);
    }
}