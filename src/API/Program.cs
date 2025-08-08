using API.Contracts;
using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Application.Users.SignUp;
using Domain.Users;
using dotenv.net;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Migration;
using Persistence;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotEnv.Load();
}

builder.Configuration.AddEnvironmentVariables();

#region Application Layer

builder.Services.AddScoped<UserSignUpHandler>();
builder.Services.AddScoped<UserSignInHandler>();

#endregion

#region Infrastructure Layer

builder.Services.AddOptions<AuthorizationTokenConfig>().BindConfiguration("AUTHORIZATION_TOKEN").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthorizationTokenService, AuthorizationTokenService>();
builder.Services.AddSingleton<AuthorizationTokenDescription>();

#endregion

#region Persistence Layer

var connectionString = builder.Configuration.GetValue<string>("CONNECTION_STRING") ?? throw new ApplicationException("CONNECTION_STRING is empty.");

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IDatabaseConnectionFactory>(new DapperConnectionFactory(connectionString));

#endregion

var app = builder.Build();

var api = app.MapGroup("/api");

api.MapPost("/sign-up", async ([FromBody]ApiSignUpContract contract, [FromServices]UserSignUpHandler handler) =>
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
});

api.MapPost("/sign-in", async ([FromBody]ApiSignInContract contract, [FromServices]UserSignInHandler handler) =>
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
});

DatabaseMigrator.MigrateDatabase(connectionString);
app.Run();