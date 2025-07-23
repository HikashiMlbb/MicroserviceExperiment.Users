using Application.Abstractions;
using Application.Users;
using Application.Users.SignUp;
using Domain.Users;
using Moq;
using NUnit.Framework;
using UserDomainErrors = Domain.Users.UserErrors;
using UserApplicationErrors = Application.Users.UserErrors;

namespace ApplicationTests.Users;

[TestFixture]
public class SignUpTests
{
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IPasswordService> _passwordMock;
    private Mock<IAuthorizationTokenService> _tokenMock;
    private UserSignUpHandler _handler;

    [SetUp]
    public void Setup()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _passwordMock = new Mock<IPasswordService>();
        _tokenMock = new Mock<IAuthorizationTokenService>();
        
        _handler = new UserSignUpHandler(_uowMock.Object, _passwordMock.Object, _tokenMock.Object);
    }
    
    [Test]
    public async Task AlreadyExists_ReturnsError()
    {
        // Arrange
        const string email = "valid@email.com";
        const string name = "SomeNormalName";
        const string password = "SomePassword";
        var dto = new UserSignUp
        {
            Email = email,
            Username = name,
            Password = password
        };

        _uowMock.Setup(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>())).ReturnsAsync(true);
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserApplicationErrors.AlreadyExists));
        _uowMock.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Hash(It.IsAny<UserRawPassword>()), Times.Never);
    }
    
    [Test]
    public async Task CreateSuccessfully_ReturnsSuccess()
    {
        // Arrange
        const string email = "valid@email.com";
        const string name = "SomeNormalName";
        const string password = "SomePassword";
        var dto = new UserSignUp
        {
            Email = email,
            Username = name,
            Password = password
        };

        var hashedPassword = new UserPassword("^s0m3-h@5h3d_p@55w0rd$");
        var newUser = new User(new UserId(15), new UserEmail(email), new UserName(name), hashedPassword);
        const string authorizationToken = "this-is-an-authorization-token";
        _uowMock.Setup(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>())).ReturnsAsync(false);
        _passwordMock.Setup(x => x.Hash(It.IsAny<UserRawPassword>())).ReturnsAsync(hashedPassword);
        _uowMock.Setup(x => x.Users.Create(It.IsAny<UserEmail>(), It.IsAny<UserName>(), It.IsAny<UserPassword>())).ReturnsAsync(newUser);
        _tokenMock.Setup(x => x.GenerateToken(It.IsAny<AuthorizationTokenPayload>())).ReturnsAsync(authorizationToken);
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!, Is.EqualTo(authorizationToken));
        _uowMock.Verify(x => x.Users.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Once);
        _passwordMock.Verify(x => x.Hash(It.IsAny<UserRawPassword>()), Times.Once);
        _uowMock.Verify(x => x.Users.Create(It.IsAny<UserEmail>(), It.IsAny<UserName>(), It.IsAny<UserPassword>()), Times.Once);
        _tokenMock.Verify(x => x.GenerateToken(It.IsAny<AuthorizationTokenPayload>()), Times.Once);
    }
}