using System.ComponentModel.DataAnnotations;

namespace AzureKeyVault1.Config;

public class AzureKeyVaultConfig
{
  [Required]
  public string VaultUri { get; set; } = default!;
  public bool UseDefaultCredential { get; set; } = true;
  public string? TenantId { get; set; }
  public string? ClientId { get; set; }
  public string? ClientSecret { get; set; }
}
