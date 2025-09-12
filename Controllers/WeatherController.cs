using Microsoft.AspNetCore.Mvc;
using WeatherMap.DTOs;
using WeatherMap.Services;
using WeatherMap.Models;

namespace WeatherMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IWeatherDatabaseService _weatherDatabaseService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(
            IWeatherService weatherService,
            IWeatherDatabaseService weatherDatabaseService,
            ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _weatherDatabaseService = weatherDatabaseService;
            _logger = logger;
        }

        /// <summary>
        /// Busca dados climáticos por cidade e salva no banco de dados
        /// </summary>
        /// <param name="cityName">Nome da cidade</param>
        /// <param name="countryCode">Código do país (opcional)</param>
        /// <param name="forecastDays">Dias de previsão (1-7)</param>
        [HttpPost("fetch-and-save/city/{cityName}")]
        public async Task<IActionResult> FetchAndSaveWeatherByCity(
            string cityName,
            [FromQuery] string? countryCode = null,
            [FromQuery] int forecastDays = 7)
        {
            try
            {
                _logger.LogInformation("Buscando e salvando dados climáticos para cidade: {City}", cityName);

                // Validação
                if (forecastDays < 1 || forecastDays > 7)
                {
                    return BadRequest(new { message = "forecastDays deve estar entre 1 e 7" });
                }

                // Buscar dados da API externa
                var weatherData = await _weatherService.GetWeatherByCityAsync(cityName, countryCode, forecastDays);

                if (weatherData == null)
                {
                    return NotFound(new { message = $"Não foi possível encontrar dados climáticos para {cityName}" });
                }

                // Salvar no banco de dados
                var savedWeather = await _weatherDatabaseService.SaveWeatherDataAsync(weatherData);

                return Ok(new
                {
                    success = true,
                    message = "Dados climáticos obtidos e salvos com sucesso",
                    savedId = savedWeather.Id,
                    location = weatherData.Location,
                    retrievedAt = weatherData.RetrievedAt,
                    data = weatherData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar e salvar dados climáticos para cidade: {City}", cityName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Busca dados climáticos por coordenadas e salva no banco de dados
        /// </summary>
        [HttpPost("fetch-and-save/coordinates")]
        public async Task<IActionResult> FetchAndSaveWeatherByCoordinates(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] int forecastDays = 7)
        {
            try
            {
                _logger.LogInformation("Buscando e salvando dados climáticos para coordenadas: {Lat}, {Lon}", latitude, longitude);

                // Validação
                if (forecastDays < 1 || forecastDays > 7)
                {
                    return BadRequest(new { message = "forecastDays deve estar entre 1 e 7" });
                }

                // Buscar dados da API externa
                var weatherData = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude, forecastDays);

                if (weatherData == null)
                {
                    return NotFound(new { message = "Não foi possível obter dados climáticos para essas coordenadas" });
                }

                // Adicionar um nome de localização baseado nas coordenadas se não existir
                if (string.IsNullOrEmpty(weatherData.Location))
                {
                    weatherData.Location = $"Lat: {latitude:F4}, Lon: {longitude:F4}";
                }

                // Salvar no banco de dados
                var savedWeather = await _weatherDatabaseService.SaveWeatherDataAsync(weatherData);

                return Ok(new
                {
                    success = true,
                    message = "Dados climáticos obtidos e salvos com sucesso",
                    savedId = savedWeather.Id,
                    coordinates = new { latitude, longitude },
                    retrievedAt = weatherData.RetrievedAt,
                    data = weatherData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar e salvar dados climáticos para coordenadas: {Lat}, {Lon}", latitude, longitude);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna o histórico de consultas climáticas salvas
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetWeatherHistory([FromQuery] int limit = 50)
        {
            try
            {
                _logger.LogInformation("Buscando histórico de dados climáticos (limit: {Limit})", limit);

                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new { message = "limit deve estar entre 1 e 100" });
                }

                var history = await _weatherDatabaseService.GetWeatherHistoryAsync(limit);

                return Ok(new
                {
                    success = true,
                    count = history.Count,
                    data = history.Select(h => new
                    {
                        id = h.Id,
                        location = h.LocationName,
                        coordinates = new { latitude = h.Latitude, longitude = h.Longitude },
                        retrievedAt = h.RetrievedAt,
                        createdAt = h.CreatedAt,
                        currentWeather = new
                        {
                            temperature = h.CurrentTemperature,
                            feelsLike = h.CurrentFeelsLike,
                            description = h.CurrentWeatherDescription,
                            humidity = h.CurrentHumidity,
                            windSpeed = h.CurrentWindSpeed
                        },
                        forecastCount = h.DailyForecasts.Count
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar histórico de dados climáticos");
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna dados climáticos salvos por ID
        /// </summary>
        [HttpGet("saved/{id:int}")]
        public async Task<IActionResult> GetSavedWeatherById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando dados climáticos salvos com ID: {Id}", id);

                var weatherData = await _weatherDatabaseService.GetWeatherByIdAsync(id);

                if (weatherData == null)
                {
                    return NotFound(new { message = $"Dados climáticos com ID {id} não encontrados" });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = weatherData.Id,
                        location = weatherData.LocationName,
                        coordinates = new { latitude = weatherData.Latitude, longitude = weatherData.Longitude },
                        timezone = weatherData.Timezone,
                        retrievedAt = weatherData.RetrievedAt,
                        createdAt = weatherData.CreatedAt,
                        current = new
                        {
                            time = weatherData.CurrentTime,
                            temperature = weatherData.CurrentTemperature,
                            feelsLike = weatherData.CurrentFeelsLike,
                            humidity = weatherData.CurrentHumidity,
                            pressure = weatherData.CurrentPressure,
                            windSpeed = weatherData.CurrentWindSpeed,
                            windDirection = weatherData.CurrentWindDirection,
                            cloudCover = weatherData.CurrentCloudCover,
                            precipitation = weatherData.CurrentPrecipitation,
                            description = weatherData.CurrentWeatherDescription,
                            isDay = weatherData.CurrentIsDay
                        },
                        dailyForecasts = weatherData.DailyForecasts.Select(f => new
                        {
                            date = f.ForecastDate,
                            temperatureMax = f.TemperatureMax,
                            temperatureMin = f.TemperatureMin,
                            precipitationSum = f.PrecipitationSum,
                            precipitationProbability = f.PrecipitationProbability,
                            windSpeedMax = f.WindSpeedMax,
                            description = f.WeatherDescription
                        }).OrderBy(f => f.date)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados climáticos salvos com ID: {Id}", id);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Busca dados climáticos salvos por nome da localização
        /// </summary>
        [HttpGet("saved/location/{locationName}")]
        public async Task<IActionResult> GetSavedWeatherByLocation(string locationName)
        {
            try
            {
                _logger.LogInformation("Buscando dados climáticos salvos para localização: {Location}", locationName);

                var weatherData = await _weatherDatabaseService.GetWeatherByLocationAsync(locationName);

                if (!weatherData.Any())
                {
                    return NotFound(new { message = $"Nenhum dado climático encontrado para {locationName}" });
                }

                return Ok(new
                {
                    success = true,
                    count = weatherData.Count,
                    location = locationName,
                    data = weatherData.Select(h => new
                    {
                        id = h.Id,
                        location = h.LocationName,
                        coordinates = new { latitude = h.Latitude, longitude = h.Longitude },
                        retrievedAt = h.RetrievedAt,
                        createdAt = h.CreatedAt,
                        currentWeather = new
                        {
                            temperature = h.CurrentTemperature,
                            feelsLike = h.CurrentFeelsLike,
                            description = h.CurrentWeatherDescription
                        }
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados climáticos por localização: {Location}", locationName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna tendências e estatísticas dos dados salvos
        /// </summary>
        [HttpGet("analytics/trends/{locationName}")]
        public async Task<IActionResult> GetWeatherTrends(string locationName, [FromQuery] int days = 30)
        {
            try
            {
                _logger.LogInformation("Analisando tendências climáticas para {Location} nos últimos {Days} dias", locationName, days);

                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-days);

                var forecasts = await _weatherDatabaseService.GetForecastsForLocationAsync(locationName, startDate, endDate);

                if (!forecasts.Any())
                {
                    return NotFound(new { message = $"Nenhum dado encontrado para {locationName} no período especificado" });
                }

                var analytics = new
                {
                    location = locationName,
                    period = new { startDate, endDate, totalDays = days },
                    dataPoints = forecasts.Count,
                    temperatureTrends = new
                    {
                        averageMax = Math.Round(forecasts.Average(f => f.TemperatureMax), 2),
                        averageMin = Math.Round(forecasts.Average(f => f.TemperatureMin), 2),
                        highest = Math.Round(forecasts.Max(f => f.TemperatureMax), 2),
                        lowest = Math.Round(forecasts.Min(f => f.TemperatureMin), 2)
                    },
                    precipitationTrends = new
                    {
                        totalPrecipitation = Math.Round(forecasts.Sum(f => f.PrecipitationSum), 2),
                        averageDaily = Math.Round(forecasts.Average(f => f.PrecipitationSum), 2),
                        rainyDays = forecasts.Count(f => f.PrecipitationSum > 0),
                        averageProbability = Math.Round(forecasts.Average(f => f.PrecipitationProbability), 2)
                    },
                    windTrends = new
                    {
                        averageWindSpeed = Math.Round(forecasts.Average(f => f.WindSpeedMax), 2),
                        maxWindSpeed = Math.Round(forecasts.Max(f => f.WindSpeedMax), 2)
                    },
                    weatherPatterns = forecasts
                        .GroupBy(f => f.WeatherDescription)
                        .Select(g => new { condition = g.Key, occurrences = g.Count() })
                        .OrderByDescending(x => x.occurrences)
                        .Take(5)
                };

                return Ok(new
                {
                    success = true,
                    analytics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao analisar tendências climáticas para {Location}", locationName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }
    }
}