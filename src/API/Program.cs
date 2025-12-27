using API.Contracts;
using API.Endpoints;
using Application.Abstractions;
using Application.ResetTokens.Request;
using Application.ResetTokens.Submit;
using Application.Users;
using Application.Users.SignIn;
using Application.Users.SignUp;
using Domain.Users;
using dotenv.net;
using Infrastructure.ResetTokens;
using Infrastructure.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Migration;
using Persistence;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotEnv.Load();
}

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHealthChecks();

#region Application Layer

builder.Services.AddScoped<UserSignUpHandler>();
builder.Services.AddScoped<UserSignInHandler>();

builder.Services.AddScoped<RequestResetTokenHandler>();
builder.Services.AddScoped<SubmitResetTokenHandler>();

#endregion

#region Infrastructure Layer

builder.Services.AddOptions<AuthorizationTokenConfig>().BindConfiguration("AUTHORIZATION_TOKEN").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ResetTokenSettings>().BindConfiguration("RESET_TOKEN").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<RabbitMqSettings>().Configure(x =>
{
    x.ConnectionString = builder.Configuration["RABBIT_MQ:CONNECTION_STRING"] ?? string.Empty;
    x.QueueName = builder.Configuration["RABBIT_MQ:QUEUE_NAME"] ?? string.Empty;
}).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthorizationTokenService, AuthorizationTokenService>();
builder.Services.AddSingleton<AuthorizationTokenDescription>();

const string emailTemplatePath = "/var/templates/email.html";
var emailTemplate = File.Exists(emailTemplatePath)
    ? await File.ReadAllTextAsync(emailTemplatePath)
    : throw new FileNotFoundException($"Email Template File is not exists. Path: {emailTemplatePath}");

builder.Services.AddScoped<IResetTokenService, ResetTokenService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<EmailTemplateService>();
builder.Services.AddSingleton<EmailTemplate>(new EmailTemplate { Template = emailTemplate });
builder.Services.AddSingleton<ResetTokenSettings>(x => x.GetRequiredService<IOptions<ResetTokenSettings>>().Value);
builder.Services.AddSingleton<RabbitMqSettings>(x => x.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

#endregion

#region Persistence Layer

var connectionString = builder.Configuration.GetValue<string>("CONNECTION_STRING") ?? throw new ApplicationException("CONNECTION_STRING is empty.");
var cacheConnectionString = builder.Configuration.GetValue<string>("CACHE_CONNECTION") ?? throw new ApplicationException("CACHE_CONNECTION is empty.");

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IDatabaseConnectionFactory>(new DapperConnectionFactory(connectionString));
builder.Services.AddStackExchangeRedisCache(setup =>
{
    setup.InstanceName = "Users.Microservice_";
    setup.Configuration = cacheConnectionString;
});

#endregion

var app = builder.Build();

app.MapHealthChecks("/healthz");

var api = app.MapGroup("/api");

api.MapUserEndpoints();
api.MapResetTokenEndpoints();

DatabaseMigrator.MigrateDatabase(connectionString);
app.Run();

public abstract partial class Program;