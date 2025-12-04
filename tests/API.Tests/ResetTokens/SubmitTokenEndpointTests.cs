using System.Net;
using System.Net.Http.Json;
using API.Contracts;
using API.Tests.Abstractions;
using StackExchange.Redis;

namespace API.Tests.ResetTokens;

public class SubmitTokenEndpointTests : TestWebAppFactory
{
    [Test]
    public async Task Token_Do_Not_Exist_Should_Return_BadRequest()
    {
        // Arrange
        
        // Act
        var result = await Http.PostAsync("/api/reset/some-token", new FormUrlEncodedContent([]));

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task New_Password_Is_Null_Should_Return_Ok()
    {
        // Arrange
        var contract = new ApiSignUpContract
        {
            Email = "npinsro@mail.com",
            Username = "Usernpinsro",
            Password = "SomePsswd"
        };
        var urlEncoded = KeyValuePair.Create("email", contract.Email);
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", contract);
        var requestTokenResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([urlEncoded]));

        var connection = await ConnectionMultiplexer.ConnectAsync(Redis.GetConnectionString());
        var db = connection.GetDatabase();
        var token = await db.HashGetAsync($"Users.Microservice_{contract.Email}", "data");
        var result = await Http.PostAsync($"/api/reset/{token.ToString()}", new FormUrlEncodedContent([]));
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requestTokenResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(token.ToString(), Is.Not.Empty);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public async Task New_Password_Is_Not_Confirmed_Should_Return_BadRequest()
    {
        // Arrange
        var contract = new ApiSignUpContract
        {
            Email = "npincsrb@mail.com",
            Username = "Usernpincsrb",
            Password = "SomePsswd"
        };
        var urlEncoded = KeyValuePair.Create("email", contract.Email);
        var submitData = new FormUrlEncodedContent([
            KeyValuePair.Create("newPassword", "SomePassword"),
            KeyValuePair.Create("confirmPassword", "PasswordSome")
        ]);
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", contract);
        var requestTokenResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([urlEncoded]));

        var connection = await ConnectionMultiplexer.ConnectAsync(Redis.GetConnectionString());
        var db = connection.GetDatabase();
        var token = (await db.HashGetAsync($"Users.Microservice_{contract.Email}", "data")).ToString();
        var result = await Http.PostAsync($"/api/reset/{token}", submitData);
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requestTokenResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(token, Is.Not.Empty);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task New_Password_Is_Not_Valid_Should_Return_BadRequest()
    {
        // Arrange
        var contract = new ApiSignUpContract
        {
            Email = "npinvsrb@mail.com",
            Username = "Usernpinvsrb",
            Password = "SomePsswd"
        };
        var urlEncoded = KeyValuePair.Create("email", contract.Email);
        var submitData = new FormUrlEncodedContent([
            KeyValuePair.Create("newPassword", "SomePasswordINVALIDSomePasswordINVALID"),
            KeyValuePair.Create("confirmPassword", "SomePasswordINVALIDSomePasswordINVALID")
        ]);
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", contract);
        var requestTokenResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([urlEncoded]));

        var connection = await ConnectionMultiplexer.ConnectAsync(Redis.GetConnectionString());
        var db = connection.GetDatabase();
        var token = (await db.HashGetAsync($"Users.Microservice_{contract.Email}", "data")).ToString();
        var result = await Http.PostAsync($"/api/reset/{token}", submitData);
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requestTokenResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(token, Is.Not.Empty);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task Change_Password_Successfully_Should_Return_Ok()
    {
        // Arrange
        var contract = new ApiSignUpContract
        {
            Email = "cpssrbok@mail.com",
            Username = "Usercpssrok",
            Password = "SomePsswd"
        };
        var urlEncoded = KeyValuePair.Create("email", contract.Email);
        const string newPassword = "ValidPassword";
        var loginContract = new ApiSignInContract
        {
            Username = contract.Username,
            Password = newPassword
        };
        var submitData = new FormUrlEncodedContent([
            KeyValuePair.Create("newPassword", newPassword),
            KeyValuePair.Create("confirmPassword", newPassword)
        ]);
        
        // Act
        var signUpResult = await Http.PostAsJsonAsync("/api/users/sign-up", contract);
        var requestTokenResult = await Http.PostAsync("/api/reset", new FormUrlEncodedContent([urlEncoded]));

        var connection = await ConnectionMultiplexer.ConnectAsync(Redis.GetConnectionString());
        var db = connection.GetDatabase();
        var token = (await db.HashGetAsync($"Users.Microservice_{contract.Email}", "data")).ToString();
        var result = await Http.PostAsync($"/api/reset/{token}", submitData);
        var loginResult = await Http.PostAsJsonAsync("/api/users/sign-in", loginContract);
        
        // Assert
        Assert.That(signUpResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requestTokenResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(token, Is.Not.Empty);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(loginResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}