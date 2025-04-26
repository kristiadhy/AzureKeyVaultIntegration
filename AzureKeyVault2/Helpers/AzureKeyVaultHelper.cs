using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using AzureKeyVault2.Config;

namespace AzureKeyVault2.Helpers;

public static class AzureKeyVaultHelper
{
  public static void LoadzureKeyVaultConfiguration(this IConfigurationBuilder configurationBuilder)
  {
    var configuration = configurationBuilder.Build(); // Temporary configuration to read settings
    var keyVaultSettings = configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultConfig>()
        ?? throw new InvalidOperationException("AzureKeyVault configuration is missing.");

    if (string.IsNullOrWhiteSpace(keyVaultSettings.VaultUri))
    {
      throw new InvalidOperationException("AzureKeyVault:VaultUri is required.");
    }

    // Use default AzureCredential if UseDefaultCredential is true
    if (keyVaultSettings.UseDefaultCredential)
    {
      configurationBuilder.AddAzureKeyVault(
          new Uri(keyVaultSettings.VaultUri),
          new DefaultAzureCredential(),
          new AzureKeyVaultConfigurationOptions
          {
            ReloadInterval = TimeSpan.FromMinutes(30)
          });
    }
    else // Otherwise, use ClientSecretCredential
    {
      if (string.IsNullOrWhiteSpace(keyVaultSettings.TenantId) ||
          string.IsNullOrWhiteSpace(keyVaultSettings.ClientId) ||
          string.IsNullOrWhiteSpace(keyVaultSettings.ClientSecret))
      {
        throw new InvalidOperationException("TenantId, ClientId, and ClientSecret are required when UseDefaultCredential is false.");
      }

      configurationBuilder.AddAzureKeyVault(
          new Uri(keyVaultSettings.VaultUri),
          new ClientSecretCredential(
              tenantId: keyVaultSettings.TenantId,
              clientId: keyVaultSettings.ClientId,
              clientSecret: keyVaultSettings.ClientSecret),
          new AzureKeyVaultConfigurationOptions
          {
            ReloadInterval = TimeSpan.FromMinutes(30)
          });
    }
  }
}
