using Azure.Security.KeyVault.Secrets;
using AzureKeyVault1.Helpers;
using Polly;
using Polly.Retry;

namespace AzureKeyVault1.Service;
public class AzureKeyVaultService
{
  private readonly SecretClient _secretClient;
  private readonly AsyncRetryPolicy _retryPolicy;
  private readonly ILogger<AzureKeyVaultService> _logger;

  public AzureKeyVaultService(IConfiguration configuration, ILogger<AzureKeyVaultService> logger)
  {
    _secretClient = AzureKeyVaultHelper.CreateSecretClient(configuration);

    // You can skip this. It's just for retrying the request in case of transient errors.
    _retryPolicy = Policy
        .Handle<Azure.RequestFailedException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
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
      var secret = await _retryPolicy.ExecuteAsync(async () =>
      {
        var response = await _secretClient.GetSecretAsync(secretKeyName); // Use this line only, if you want to retrieve the secret value directly without applying a retry policy.
        return response.Value.Value;
      });
      _logger.LogInformation("Successfully retrieved secret {SecretKeyName}.", secretKeyName);
      return secret;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve secret {SecretKeyName} from Key Vault.", secretKeyName);
      throw;
    }
  }
}