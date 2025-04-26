using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVault1.Config;

namespace AzureKeyVault1.Helpers;

public static class AzureKeyVaultHelper
{
  public static SecretClient CreateSecretClient(IConfiguration configuration)
  {
    // Bind the configuration section to the AzureKeyVaultConfig class
    var keyVaultSetting = configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultConfig>()
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
}

