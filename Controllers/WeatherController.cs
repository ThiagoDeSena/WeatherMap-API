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

        // Métodos adicionais para atualizar, deletar e análises avançadas:

        /// <summary>
        /// Atualiza o nome da localização de um registro climático
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="locationName">Novo nome da localização</param>
        [HttpPut("saved/{id:int}/location")]
        public async Task<IActionResult> UpdateWeatherLocationName(int id, [FromBody] UpdateLocationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.LocationName))
                {
                    return BadRequest(new { message = "Nome da localização é obrigatório" });
                }

                _logger.LogInformation("Atualizando localização do registro {Id} para {Location}", id, request.LocationName);

                var updatedWeather = await _weatherDatabaseService.UpdateWeatherHistoryAsync(id, request.LocationName);

                if (updatedWeather == null)
                {
                    return NotFound(new { message = $"Registro com ID {id} não encontrado" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Localização atualizada com sucesso",
                    data = new
                    {
                        id = updatedWeather.Id,
                        oldLocation = updatedWeather.LocationName,
                        newLocation = request.LocationName,
                        updatedAt = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar localização do registro {Id}", id);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um registro climático específico por ID
        /// </summary>
        /// <param name="id">ID do registro a ser deletado</param>
        [HttpDelete("saved/{id:int}")]
        public async Task<IActionResult> DeleteWeatherHistory(int id)
        {
            try
            {
                _logger.LogInformation("Tentando deletar registro climático com ID: {Id}", id);

                var deleted = await _weatherDatabaseService.DeleteWeatherHistoryAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = $"Registro com ID {id} não encontrado" });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Registro climático {id} deletado com sucesso",
                    deletedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar registro climático {Id}", id);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta registros climáticos antigos (limpeza de dados)
        /// </summary>
        /// <param name="daysOld">Deletar registros mais antigos que X dias</param>
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupOldWeatherData([FromQuery] int daysOld = 90)
        {
            try
            {
                if (daysOld <= 0)
                {
                    return BadRequest(new { message = "daysOld deve ser maior que 0" });
                }

                _logger.LogInformation("Iniciando limpeza de dados climáticos mais antigos que {Days} dias", daysOld);

                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                var deleted = await _weatherDatabaseService.DeleteOldWeatherDataAsync(cutoffDate);

                if (!deleted)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Nenhum registro antigo encontrado para deletar",
                        cutoffDate = cutoffDate
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Registros climáticos anteriores a {cutoffDate:yyyy-MM-dd} foram deletados",
                    cutoffDate = cutoffDate,
                    deletedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na limpeza de dados antigos ({Days} dias)", daysOld);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Estatísticas de localizações usando consulta SQL bruta
        /// </summary>
        /// <param name="days">Período em dias para análise</param>
        [HttpGet("analytics/locations-stats-raw")]
        public async Task<IActionResult> GetLocationStatisticsRaw([FromQuery] int days = 30)
        {
            try
            {
                if (days <= 0 || days > 365)
                {
                    return BadRequest(new { message = "days deve estar entre 1 e 365" });
                }

                _logger.LogInformation("Buscando estatísticas de localização via SQL bruta ({Days} dias)", days);

                var stats = await _weatherDatabaseService.GetLocationStatisticsRawAsync(days);

                if (!stats.Any())
                {
                    return NotFound(new { message = $"Nenhum dado encontrado para os últimos {days} dias" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Estatísticas obtidas via consulta SQL bruta",
                    period = new { days, startDate = DateTime.UtcNow.AddDays(-days), endDate = DateTime.UtcNow },
                    totalLocations = stats.Count,
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de estatísticas");
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Tendências de temperatura usando consulta SQL bruta
        /// </summary>
        /// <param name="locationName">Nome da localização</param>
        /// <param name="days">Período em dias</param>
        [HttpGet("analytics/temperature-trends-raw/{locationName}")]
        public async Task<IActionResult> GetTemperatureTrendsRaw(string locationName, [FromQuery] int days = 30)
        {
            try
            {
                if (days <= 0 || days > 365)
                {
                    return BadRequest(new { message = "days deve estar entre 1 e 365" });
                }

                _logger.LogInformation("Buscando tendências de temperatura via SQL bruta - {Location} ({Days} dias)", locationName, days);

                var trends = await _weatherDatabaseService.GetTemperatureTrendsRawAsync(locationName, days);

                if (!trends.Any())
                {
                    return NotFound(new { message = $"Nenhum dado de tendência encontrado para {locationName}" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Tendências obtidas via consulta SQL bruta",
                    location = locationName,
                    period = new { days, dataPoints = trends.Count },
                    data = trends
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de tendências para {Location}", locationName);
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Comparação entre múltiplas localizações usando consulta SQL bruta
        /// </summary>
        /// <param name="request">Lista de localizações para comparar</param>
        [HttpPost("analytics/location-comparison-raw")]
        public async Task<IActionResult> GetLocationComparisonRaw([FromBody] LocationComparisonRequest request)
        {
            try
            {
                if (request?.LocationNames == null || !request.LocationNames.Any())
                {
                    return BadRequest(new { message = "Lista de localizações é obrigatória" });
                }

                if (request.LocationNames.Count > 10)
                {
                    return BadRequest(new { message = "Máximo de 10 localizações permitido" });
                }

                _logger.LogInformation("Comparando localizações via SQL bruta: {Locations}", string.Join(", ", request.LocationNames));

                var comparison = await _weatherDatabaseService.GetLocationComparisonRawAsync(request.LocationNames);

                if (!comparison.Any())
                {
                    return NotFound(new { message = "Nenhum dado encontrado para as localizações especificadas" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Comparação obtida via consulta SQL bruta",
                    requestedLocations = request.LocationNames,
                    foundLocations = comparison.Count,
                    data = comparison.OrderByDescending(c => c.AvgCurrentTemp)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de comparação");
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        /// <summary>
        /// Informações de saúde do banco de dados usando consulta SQL bruta
        /// </summary>
        /// /// <returns>Dados de saúde do banco incluindo contagens e estatísticas</returns>
        /// <remarks>
        /// Esta consulta executa SQL bruto para obter métricas de performance
        /// e utilização do banco de dados de forma eficiente.
        /// </remarks>
        /// <exception cref="Exception">Erro na conexão com o banco</exception>
        [HttpGet("analytics/database-health-raw")]
        public async Task<IActionResult> GetDatabaseHealthRaw()
        {
            try
            {
                _logger.LogInformation("Verificando saúde do banco via SQL bruta");

                var health = await _weatherDatabaseService.GetDatabaseHealthRawAsync();

                return Ok(new
                {
                    success = true,
                    message = "Informações de saúde obtidas via consulta SQL bruta",
                    timestamp = DateTime.UtcNow,
                    data = health
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de saúde do banco");
                return StatusCode(500, new { error = "Erro interno", message = ex.Message });
            }
        }

        // DTOs para requests
        public class UpdateLocationRequest
        {
            public string LocationName { get; set; } = string.Empty;
        }

        public class LocationComparisonRequest
        {
            public List<string> LocationNames { get; set; } = new();
        }
    }
}