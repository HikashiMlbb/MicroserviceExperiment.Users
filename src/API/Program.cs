using API.Contracts;
using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Application.Users.SignUp;
using dotenv.net;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Persistence;

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

    if (!result.IsSuccess)
    {
        if (result.Error == UserErrors.AlreadyExists) return Results.Conflict();

        return Results.BadRequest();
    }
    
    return Results.Ok(result.Value);
});

api.MapPost("/sign-in", async ([FromBody]ApiSignInContract contract, [FromServices]UserSignInHandler handler) =>
{
    var dto = new UserSignIn
    {
        Username = contract.Username,
        Password = contract.Password
    };
    var result = await handler.Handle(dto);
    
    if (!result.IsSuccess)
    {
        if (result.Error == UserErrors.NotFound || result.Error == UserErrors.LoginFailed)
        {
            return Results.Unauthorized();
        }

        return Results.BadRequest();
    }

    return Results.Ok(result.Value);

});

app.Run();