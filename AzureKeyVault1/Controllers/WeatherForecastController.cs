using AzureKeyVault1.Service;
using Microsoft.AspNetCore.Mvc;

namespace AzureKeyVault1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private readonly JwtSecretProviderService _jwtTokenProvider;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, JwtSecretProviderService jwtTokenProvider)
    {
      _logger = logger;
      _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetJwtSecretKey()
    {
      var secret = await _jwtTokenProvider.GetSecretAsync();
      return Ok(secret);
    }
  }
}
