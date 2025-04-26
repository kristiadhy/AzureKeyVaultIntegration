namespace AzureKeyVault1.Service;
public class JwtSecretProviderService
{
  private readonly IConfiguration _configuration;
  private readonly AzureKeyVaultService _keyVaultService;

  public JwtSecretProviderService(AzureKeyVaultService keyVaultService, IConfiguration configuration)
  {
    _keyVaultService = keyVaultService;
    _configuration = configuration;
  }

  public async Task<string> GetSecretAsync()
  {
    return await _keyVaultService.GetJwtSecretAsync(_configuration);
  }
}

