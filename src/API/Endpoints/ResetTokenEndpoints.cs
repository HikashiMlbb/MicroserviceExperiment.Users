using API.Contracts;
using Application.ResetTokens;
using Application.ResetTokens.Request;
using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ResetTokenEndpoints
{
    public static void MapResetTokenEndpoints(this IEndpointRouteBuilder app, string? prefix = "")
    {
        var api = app.MapGroup($"{prefix}/reset");

        api.MapPost("/", RequestToken)
            .Accepts<string>("application/x-www-form-urlencoded")
            .DisableAntiforgery();
        api.MapGet("/{token}", ([FromRoute]string token) => $"Here will be checking for reset token is valid. {token}");
        api.MapPost("/{token}", ([FromRoute]string token) => $"Here will be changing password.");
    }

    #region Endpoint implementation
    
    private static async Task<IResult> RequestToken([FromForm]string? email, [FromServices]RequestResetTokenHandler handler)
    {
        var dto = new RequestResetToken { Email = email };
        var result = await handler.Handle(dto);

        return result.IsSuccess ? Results.Ok() : result.Error switch
        {
            UserApplicationError err => Results.NotFound(new { err.Code, err.Message }),
            ResetTokenError err when err == ResetTokenErrors.ValidationError => Results.BadRequest(new { err.Code, err.Message }),
            ResetTokenError err when err == ResetTokenErrors.AlreadyRequested => Results.Conflict(new { err.Code, err.Message }),
            _ => Results.Problem(
                 title: result.Error!.Code,
                 statusCode: StatusCodes.Status500InternalServerError,
                 detail: result.Error!.Message)
        };
    }

    #endregion
}