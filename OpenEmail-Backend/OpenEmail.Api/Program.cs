using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using OpenEmail.Api.Middlewares;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(x => x.AddMaps(System.Reflection.Assembly.GetExecutingAssembly()));
builder.Services.AddInfrastructure();
builder.Services.AddCustomExceptionHandler(options =>
{
    options.LogStructuredException = true;
    options.UseGenericReason = false; // Set to true in production to hide actual exception messages
    options.IncludeExceptionType = true; // Set to false in production
    options.IncludeRoute = true;
});
builder.Services.AddFastEndpoints()
    .AddResponseCaching()
    .SwaggerDocument(opt =>
        opt.DocumentSettings = settings =>
        {
            settings.Title = "OpenEmail API";
            settings.Version = "v1";
            settings.Description = "An API for managing emails";
        }
    );

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new ArgumentNullException($"JWT_SECRET");
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtSecret);

builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
await db.Database.MigrateAsync();

app.UseResponseCaching();
app.UseCustomExceptionHandler();
app.UseFastEndpoints().UseSwaggerGen();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();