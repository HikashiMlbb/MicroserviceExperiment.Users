using System.Net;
using System.Net.Http.Json;
using API.Contracts;
using API.Tests.Abstractions;

namespace API.Tests.ResetTokens;

[TestFixture]
public class RequestTokenEndpointTests : TestWebAppFactory
{
    [Test]
    public async Task Invalid_Email_Should_Return_BadRequest()
    {
        // Arrange
        const string email = "someinv@lid!@mail.com";
        var pair = KeyValuePair.Create("email", email);
        
        // Act
        var httpResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([pair]));

        // Assert
        Assert.That(httpResult.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task Unregistered_Should_Return_NotFound()
    {
        // Arrange
        const string email = "some_valid@mail.com";
        var pair = KeyValuePair.Create("email", email);
        
        // Act
        var httpResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([pair]));

        // Assert
        Assert.That(httpResult.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task Registered_Requested_Twice_Should_Return_Conflict()
    {
        // Arrange
        const string email = "rrtsrcsdome_valid@mail.com";
        var pair = KeyValuePair.Create("email", email);
        var account = new ApiSignUpContract
        {
            Email = email,
            Username = "rrtsrcSomeValid",
            Password = "S0m3P@ssw0rd"
        };
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", account);
        var httpResult1 = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([pair]));
        var httpResult2 = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([pair]));
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(httpResult1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(httpResult2.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
    
    [Test]
    public async Task Registered_Requested_Twice_Should_Return_OK()
    {
        // Arrange
        const string email = "rrtsroksome_valid@mail.com";
        var pair = KeyValuePair.Create("email", email);
        var account = new ApiSignUpContract
        {
            Email = email,
            Username = "rrtsrokSomeValid",
            Password = "S0m3P@ssw0rd"
        };
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", account);
        var httpResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([pair]));
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(httpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}