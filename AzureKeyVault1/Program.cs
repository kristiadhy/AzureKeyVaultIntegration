using AzureKeyVault1.Config;
using AzureKeyVault1.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSetting"));

services.AddSingleton<AzureKeyVaultService>(); // You can call the Jwt secret using this service

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
