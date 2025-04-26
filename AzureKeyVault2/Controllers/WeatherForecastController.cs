using Microsoft.AspNetCore.Mvc;

namespace AzureKeyVault2.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetJwtSecretKey()
    {
      var secret = await Task.FromResult(_configuration["JwtSettings:Secret"]);
      return Ok(secret);
    }

  }
}
