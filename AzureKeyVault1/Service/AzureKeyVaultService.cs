using Azure.Security.KeyVault.Secrets;
using AzureKeyVault1.Helpers;

namespace AzureKeyVault1.Service;
public class AzureKeyVaultService
{
  private readonly SecretClient _secretClient;
  private readonly ILogger<AzureKeyVaultService> _logger;

  public AzureKeyVaultService(IConfiguration configuration, ILogger<AzureKeyVaultService> logger)
  {
    _secretClient = AzureKeyVaultHelper.CreateSecretClient(configuration);
    _logger = logger;
  }

  public async Task<string> GetJwtSecretAsync()
  {
    // In Azure Key Vault, I named the secret as "JwtSettings--Secret" (It could be anything you want). 
    // It's the same as "JwtSettings:Secret" in the application configuration (e.g., appsettings.json).
    // This way, when loading secrets from Key Vault, it will automatically map to "JwtSettings:Secret" in the configuration. (You can see in the AzureKeyVault1 project)

    string secretKeyName = "JwtSettings--Secret";
    try
    {
      _logger.LogInformation("Retrieving secret {SecretKeyName} from Key Vault.", secretKeyName);
      var secret = await _secretClient.GetSecretAsync(secretKeyName);
      _logger.LogInformation("Successfully retrieved secret {SecretKeyName}.", secretKeyName);
      return secret.Value.Value;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve secret {SecretKeyName} from Key Vault.", secretKeyName);
      throw;
    }
  }
}