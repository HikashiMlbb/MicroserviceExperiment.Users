using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ResetTokenEndpoints
{
    public static void MapResetTokenEndpoints(this IEndpointRouteBuilder app, string? prefix = "")
    {
        var api = app.MapGroup($"{prefix}/reset");

        api.MapPost("/", () => "Here will be method that generates and publishes reset token.");
        api.MapGet("/{token}", ([FromRoute]string token) => $"Here will be checking for reset token is valid. {token}");
        api.MapPost("/{token}", ([FromRoute]string token) => $"Here will be changing password.");
    } 
}