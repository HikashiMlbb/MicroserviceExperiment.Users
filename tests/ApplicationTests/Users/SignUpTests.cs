using Application.Abstractions;
using Application.Users;
using Application.Users.SignUp;
using Domain.Users;
using Moq;
using NUnit.Framework;
using UserDomainErrors = Domain.Users.UserErrors;

namespace ApplicationTests.Users;

[TestFixture]
public class SignUpTests
{
    private Mock<IUserRepository> _repoMock;
    private Mock<IPasswordService> _passwordMock;
    private Mock<IAuthorizationTokenService> _tokenMock;
    private UserSignUpHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IUserRepository>();
        _passwordMock = new Mock<IPasswordService>();
        _tokenMock = new Mock<IAuthorizationTokenService>();
        
        _handler = new UserSignUpHandler(_repoMock.Object, _passwordMock.Object, _tokenMock.Object);
    }

    [Test]
    public async Task InvalidEmail_ReturnsError()
    {
        // Arrange
        const string email = "invalid @email.comme";
        const string name = "SomeName";
        const string password = "SomePassword's password";
        var dto = new UserSignUp
        {
            Email = email,
            Username = name,
            Password = password
        };
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserDomainErrors.EmailIsInvalid));
        _repoMock.Verify(x => x.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Never);
    }
    
    [Test]
    public async Task InvalidUsername_ReturnsError()
    {
        // Arrange
        const string email = "valid@email.com";
        const string name = "Some Strange Name";
        const string password = "SomePassword's password";
        var dto = new UserSignUp
        {
            Email = email,
            Username = name,
            Password = password
        };
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserDomainErrors.UsernameIsInvalid));
        _repoMock.Verify(x => x.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Never);
    }
    
    [Test]
    public async Task InvalidPassword_ReturnsError()
    {
        // Arrange
        const string email = "valid@email.com";
        const string name = "SomeNormalName";
        const string password = "SomePassword's too large password";
        var dto = new UserSignUp
        {
            Email = email,
            Username = name,
            Password = password
        };
        
        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error!, Is.EqualTo(UserDomainErrors.PasswordIsOutOfRange));
        _repoMock.Verify(x => x.IsExists(It.IsAny<UserEmail>(), It.IsAny<UserName>()), Times.Never);
    }
}