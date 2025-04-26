using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace BusinessLogic.Utility;

public static class AzureKeyVaultHelper
{
  public static SecretClient CreateSecretClient(IConfiguration configuration)
  {
    // Bind the configuration section to the AzureKeyVaultSetting class
    var keyVaultSetting = configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultSetting>()
               ?? throw new InvalidOperationException("Missing AzureKeyVault configuration.");

    if (string.IsNullOrWhiteSpace(keyVaultSetting.VaultUri))
      throw new InvalidOperationException("VaultUri must be provided.");

    var vaultUri = new Uri(keyVaultSetting.VaultUri);

    if (keyVaultSetting.UseDefaultCredential)
    {
      return new SecretClient(vaultUri, new DefaultAzureCredential());
    }

    // Validate ClientSecret credentials
    if (string.IsNullOrWhiteSpace(keyVaultSetting.ClientId) ||
        string.IsNullOrWhiteSpace(keyVaultSetting.ClientSecret) ||
        string.IsNullOrWhiteSpace(keyVaultSetting.TenantId))
    {
      throw new InvalidOperationException("ClientId, ClientSecret, and TenantId are required when not using DefaultAzureCredential.");
    }

    var credential = new ClientSecretCredential(keyVaultSetting.TenantId, keyVaultSetting.ClientId, keyVaultSetting.ClientSecret);
    return new SecretClient(vaultUri, credential);
  }

  public static void AddAzureKeyVaultConfiguration(this IConfigurationBuilder configurationBuilder)
  {
    var configuration = configurationBuilder.Build(); // Temporary configuration to read settings
    var keyVaultSettings = configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultSetting>()
        ?? throw new InvalidOperationException("AzureKeyVault configuration is missing.");

    if (string.IsNullOrWhiteSpace(keyVaultSettings.VaultUri))
    {
      throw new InvalidOperationException("AzureKeyVault:VaultUri is required.");
    }

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
    else
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

