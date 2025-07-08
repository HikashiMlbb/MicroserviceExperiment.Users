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
}