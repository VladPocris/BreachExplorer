using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using API_Proxy.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
Env.Load();

// Load environment variables from .env file
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Add HttpClient service for dependency injection
builder.Services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();
builder.Services.AddScoped<IBreachCheckService, BreachCheckService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Proxy API",
            Version = "v1"
        });
    });
}

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowAll");

// Use HTTPS redirection and authorization (if needed)
app.UseHttpsRedirection();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proxy API v1");
    });
}

// Start the application
app.Run();