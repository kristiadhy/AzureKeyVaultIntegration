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

    _retryPolicy = Policy
        .Handle<Azure.RequestFailedException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    _logger = logger;
  }

  public async Task<string> GetJwtSecretAsync(IConfiguration configuration)
  {
    string secretKeyName = configuration["JwtSettings:Secret"];
    if (string.IsNullOrWhiteSpace(secretKeyName))
    {
      _logger.LogError("The secret key name is not configured properly.");
      throw new InvalidOperationException("The secret key name is not configured properly");
    }

    if (string.IsNullOrEmpty(secretKeyName))
    {
      _logger.LogError("The secret key name is not configured properly.");
      throw new InvalidOperationException("The secret key name is not configured properly");
    }

    try
    {
      _logger.LogInformation("Retrieving secret {SecretKeyName} from Key Vault.", secretKeyName);
      var secret = await _retryPolicy.ExecuteAsync(async () =>
      {
        var secret = await _secretClient.GetSecretAsync(secretKeyName);
        return secret.Value.Value;
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