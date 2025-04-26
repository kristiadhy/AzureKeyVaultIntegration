using AzureKeyVault2.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.LoadzureKeyVaultConfiguration(); // Custom extension method to add Azure Key Vault configuration

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
