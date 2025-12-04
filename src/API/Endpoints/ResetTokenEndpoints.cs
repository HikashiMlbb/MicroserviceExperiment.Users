using API.Contracts;
using Application.ResetTokens;
using Application.ResetTokens.Request;
using Application.ResetTokens.Submit;
using Application.Users;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace API.Endpoints;

public static class ResetTokenEndpoints
{
    public static void MapResetTokenEndpoints(this IEndpointRouteBuilder app, string? prefix = "")
    {
        var api = app.MapGroup($"{prefix}/reset");

        api.MapPost("/", RequestToken)
            .Accepts<string>("application/x-www-form-urlencoded")
            .DisableAntiforgery();
        api.MapPost("/{token}", SubmitToken)
            .Accepts<string>("application/x-www-form-urlencoded")
            .DisableAntiforgery();
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

    private static async Task<IResult> SubmitToken(
        [FromRoute]string token, 
        [FromForm]string? newPassword,
        [FromForm]string? confirmPassword,
        [FromServices]SubmitResetTokenHandler handler)
    {
        var dto = new SubmitResetToken
        {
            Token = token,
            NewPassword = newPassword,
            ConfirmPassword = confirmPassword
        };
        
        var result = await handler.Handle(dto);

        return result.IsSuccess ? Results.Ok() : result.Error switch
        {
            ResetTokenError err when err == ResetTokenErrors.Empty => Results.BadRequest(new { err.Code, err.Message }),
            ResetTokenError err when err == ResetTokenErrors.NotExistsOrExpired => Results.NotFound(new { err.Code, err.Message }),
            UserApplicationError err => Results.BadRequest(new { err.Code, err.Message }),
            UserDomainError err => Results.BadRequest(new { err.Code, err.Message }),
            _ => Results.Problem(
                title: result.Error!.Code,
                statusCode: StatusCodes.Status500InternalServerError,
                detail: result.Error.Message)
        } ;
    }

    #endregion
}