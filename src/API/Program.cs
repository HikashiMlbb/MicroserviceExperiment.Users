var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var api = app.MapGroup("/api");

api.MapPost("/sign-up", () =>
{
    return Results.Ok();
});

app.Run();
