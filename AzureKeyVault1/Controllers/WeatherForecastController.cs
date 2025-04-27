using AzureKeyVault1.Service;
using Microsoft.AspNetCore.Mvc;

namespace AzureKeyVault1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private readonly AzureKeyVaultService _azureKeyVaultService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, AzureKeyVaultService azureKeyVaultService)
    {
      _logger = logger;
      _azureKeyVaultService = azureKeyVaultService;
    }

    [HttpGet]
    public async Task<IActionResult> GetJwtSecretKey()
    {
      var secret = await _azureKeyVaultService.GetJwtSecretAsync();
      return Ok(secret);
    }
  }
}
