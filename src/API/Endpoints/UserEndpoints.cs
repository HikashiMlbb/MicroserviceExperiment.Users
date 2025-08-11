namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app, string? prefix = "")
    {
        var api = app.MapGroup($"{prefix}/users");
    }
}