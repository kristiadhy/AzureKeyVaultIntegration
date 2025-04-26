namespace AzureKeyVault1.Config;

public class JwtSetting
{
  public string Issuer { get; set; }
  public string Audience { get; set; }
  public int AccessTokenExpMinute { get; set; }
  public int RefreshTokenExpiration { get; set; }
}
