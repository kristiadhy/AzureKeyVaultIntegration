namespace AzureKeyVault1.Service;
public class JwtSecretProviderService
{
  private readonly AzureKeyVaultService _keyVaultService;

  public JwtSecretProviderService(AzureKeyVaultService keyVaultService)
  {
    _keyVaultService = keyVaultService;
  }

  public async Task<string> GetSecretAsync()
  {
    return await _keyVaultService.GetJwtSecretAsync();
  }
}

