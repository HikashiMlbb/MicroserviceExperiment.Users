using API.Contracts;
using API.Endpoints;
using Application.Abstractions;
using Application.Users;
using Application.Users.SignIn;
using Application.Users.SignUp;
using Domain.Users;
using dotenv.net;
using Infrastructure.Users;
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

api.MapUserEndpoints();
api.MapResetTokenEndpoints();

DatabaseMigrator.MigrateDatabase(connectionString);
app.Run();