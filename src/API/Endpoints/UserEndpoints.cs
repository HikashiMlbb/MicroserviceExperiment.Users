using API.Contracts;
using Application.Users;
using Application.Users.SignIn;
using Application.Users.SignUp;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app, string? prefix = "")
    {
        var api = app.MapGroup($"{prefix}/users");

        api.MapPost("/sign-up", SignUp);
        api.MapPost("/sign-in", SignIn);
    }

    #region Endpoint implementation

    private static async Task<IResult> SignUp(
        [FromBody] ApiSignUpContract contract,
        [FromServices] UserSignUpHandler handler)
    {
        var dto = new UserSignUp
        {
            Email = contract.Email,
            Username = contract.Username,
            Password = contract.Password
        };
        var result = await handler.Handle(dto);

        if (result.IsSuccess) return Results.Ok(result.Value);

        return result.Error switch
        {
            UserDomainError domain => Results.BadRequest(new { domain.Code, domain.Message }),
            UserApplicationError application => Results.Conflict(new { application.Code, application.Message }),
            _ => Results.StatusCode(500)
        };
    }
    
    private static async Task<IResult> SignIn([FromBody]ApiSignInContract contract, [FromServices]UserSignInHandler handler)
    {
        var dto = new UserSignIn
        {
            Username = contract.Username,
            Password = contract.Password
        };
        var result = await handler.Handle(dto);

        if (result.IsSuccess) return Results.Ok(result.Value);

        return result.Error switch
        {
            UserApplicationError => Results.Unauthorized(),
            UserDomainError domain => Results.BadRequest(new { domain.Code, domain.Message }),
            _ => Results.StatusCode(500)
        };
    }

    #endregion
}