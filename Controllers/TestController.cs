using Microsoft.AspNetCore.Mvc;
using WeatherMap.Services;

namespace WeatherMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<TestController> _logger;

        public TestController(IWeatherService weatherService, ILogger<TestController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        /// <summary>
        /// Testa se a API externa está funcionando
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                var isHealthy = await _weatherService.IsApiHealthyAsync();
                return Ok(new
                {
                    status = isHealthy ? "Healthy" : "Unhealthy",
                    message = isHealthy ? "API Open-Meteo está respondendo" : "API Open-Meteo não está respondendo",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no health check");
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Busca clima por nome da cidade (apenas teste da API externa)
        /// </summary>
        [HttpGet("weather/city/{cityName}")]
        public async Task<IActionResult> GetWeatherByCity(string cityName, string? countryCode = null)
        {
            try
            {
                _logger.LogInformation("Testando busca de clima para cidade: {City}", cityName);

                // Primeiro, testar o geocoding separadamente
                var locations = await _weatherService.GetLocationsByNameAsync(cityName, countryCode);

                if (!locations.Any())
                {
                    _logger.LogWarning("Nenhuma localização encontrada via geocoding para: {City}", cityName);
                    return NotFound(new
                    {
                        message = $"Nenhuma localização encontrada para {cityName}",
                        step = "geocoding_failed"
                    });
                }

                var location = locations.First();
                _logger.LogInformation("Localização encontrada: {Name} ({Lat}, {Lon})", location.Name, location.Latitude, location.Longitude);

                // Agora testar o forecast
                var weather = await _weatherService.GetWeatherByCoordinatesAsync(location.Latitude, location.Longitude);

                if (weather == null)
                {
                    _logger.LogWarning("Falha ao buscar dados climáticos para coordenadas: {Lat}, {Lon}", location.Latitude, location.Longitude);
                    return BadRequest(new
                    {
                        message = "Geocoding funcionou, mas falha ao buscar dados climáticos",
                        step = "forecast_failed",
                        location = location
                    });
                }

                // Adicionar informações da localização
                weather.Location = $"{location.Name}, {location.Country}";

                return Ok(new
                {
                    success = true,
                    message = "Dados obtidos com sucesso da API Open-Meteo",
                    steps = new { geocoding = "success", forecast = "success" },
                    data = weather
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clima por cidade: {City}", cityName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Busca clima por nome da cidade (apenas teste da API externa)
        /// </summary>
        // [HttpGet("weather/city/{cityName}")]
        // public async Task<IActionResult> GetWeatherByCity(string cityName, string? countryCode = null)
        // {
        //     try
        //     {
        //         _logger.LogInformation("Testando busca de clima para cidade: {City}", cityName);

        //         var weather = await _weatherService.GetWeatherByCityAsync(cityName, countryCode);

        //         if (weather == null)
        //         {
        //             return NotFound(new { message = $"Não foi possível encontrar dados climáticos para {cityName}" });
        //         }

        //         return Ok(new
        //         {
        //             success = true,
        //             message = "Dados obtidos com sucesso da API Open-Meteo",
        //             data = weather
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Erro ao buscar clima por cidade: {City}", cityName);
        //         return StatusCode(500, new { error = "Erro interno", message = ex.Message });
        //     }
        // }

        /// <summary>
        /// Busca clima por coordenadas (apenas teste da API externa)
        /// </summary>
        [HttpGet("weather/coordinates")]
        public async Task<IActionResult> GetWeatherByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                _logger.LogInformation("Testando busca de clima para coordenadas: {Lat}, {Lon}", latitude, longitude);

                var weather = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude);

                if (weather == null)
                {
                    return NotFound(new { message = "Não foi possível obter dados climáticos para essas coordenadas" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Dados obtidos com sucesso da API Open-Meteo",
                    data = weather
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clima por coordenadas: {Lat}, {Lon}", latitude, longitude);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Busca coordenadas por nome da cidade
        /// </summary>
        [HttpGet("geocoding/{cityName}")]
        public async Task<IActionResult> GetLocationsByName(string cityName, string? countryCode = null)
        {
            try
            {
                _logger.LogInformation("Testando busca de coordenadas para: {City}", cityName);

                var locations = await _weatherService.GetLocationsByNameAsync(cityName, countryCode);

                if (!locations.Any())
                {
                    return NotFound(new { message = $"Nenhuma localização encontrada para {cityName}" });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Encontradas {locations.Count} localizações",
                    data = locations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar localizações: {City}", cityName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        [HttpGet("weather/{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            var weather = await _weatherService.GetWeatherByCityAsync(city);
            return weather != null ? Ok(weather) : NotFound();
        }


    }
}