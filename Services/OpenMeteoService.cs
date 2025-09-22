using System.Text.Json;
using Microsoft.Extensions.Options;
using WeatherMap.Configurations;
using WeatherMap.DTOs;

namespace WeatherMap.Services
{
    public class OpenMeteoService : IWeatherService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _geocodingClient; // Cliente separado para geocoding
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<OpenMeteoService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public OpenMeteoService(
            HttpClient httpClient,
            IHttpClientFactory httpClientFactory, // Use IHttpClientFactory
            IOptions<ApiSettings> apiSettings,
            ILogger<OpenMeteoService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configurações para deserialização JSON
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Configurar HttpClient para Weather API
            _httpClient.BaseAddress = new Uri(_apiSettings.OpenMeteoBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _apiSettings.UserAgent);

            // Criar cliente separado para Geocoding
            _geocodingClient = httpClientFactory.CreateClient();
            _geocodingClient.BaseAddress = new Uri(_apiSettings.GeocodingBaseUrl);
            _geocodingClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutSeconds);
            _geocodingClient.DefaultRequestHeaders.Add("User-Agent", _apiSettings.UserAgent);
        }

        /// <inheritdoc />
        public async Task<WeatherResponse?> GetWeatherByCoordinatesAsync(
            double latitude,
            double longitude,
            int forecastDays = 7,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Buscando dados climáticos para coordenadas: {Lat}, {Lon}", latitude, longitude);

                var url = BuildWeatherUrl(latitude, longitude, forecastDays);
                _logger.LogInformation("URL completa do forecast: {BaseAddress}{Url}", _httpClient.BaseAddress, url);

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Erro na API Open-Meteo: {StatusCode} - {ReasonPhrase} - Content: {Content}",
                        response.StatusCode, response.ReasonPhrase, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Resposta recebida (primeiros 200 chars): {Content}",
                    jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent);

                var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(jsonContent, _jsonOptions);

                if (openMeteoResponse == null)
                {
                    _logger.LogError("Falha ao deserializar resposta da API Open-Meteo");
                    return null;
                }

                return MapToWeatherResponse(openMeteoResponse);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao acessar API Open-Meteo");
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout ao acessar API Open-Meteo");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao deserializar resposta JSON da API Open-Meteo");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar dados climáticos");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<WeatherResponse?> GetWeatherByCityAsync(
            string cityName,
            string? countryCode = null,
            int forecastDays = 7,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Buscando dados climáticos para cidade: {City}", cityName);

                // Primeiro, buscar as coordenadas da cidade
                var locations = await GetLocationsByNameAsync(cityName, countryCode, cancellationToken);

                if (!locations.Any())
                {
                    _logger.LogWarning("Nenhuma localização encontrada para: {City}", cityName);
                    return null;
                }

                var location = locations.First();
                var weatherResponse = await GetWeatherByCoordinatesAsync(
                    location.Latitude,
                    location.Longitude,
                    forecastDays,
                    cancellationToken);

                if (weatherResponse != null)
                {
                    // Adicionar informações da cidade
                    weatherResponse.Location = $"{location.Name}, {location.Country}";
                }

                return weatherResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados climáticos por nome da cidade: {City}", cityName);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<List<LocationResult>> GetLocationsByNameAsync(
            string cityName,
            string? countryCode = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Buscando coordenadas para cidade: {City}", cityName);

                // Usar URL relativa com o cliente configurado
                var geocodingUrl = $"/v1/search?name={Uri.EscapeDataString(cityName)}&count=10&language=pt&format=json";

                if (!string.IsNullOrEmpty(countryCode))
                {
                    geocodingUrl += $"&country={countryCode}";
                }

                _logger.LogInformation("URL completa de geocoding: {BaseAddress}{Url}", _geocodingClient.BaseAddress, geocodingUrl);

                var response = await _geocodingClient.GetAsync(geocodingUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Erro na API de Geocoding: {StatusCode} - {ReasonPhrase} - Content: {Content}",
                        response.StatusCode, response.ReasonPhrase, errorContent);
                    return new List<LocationResult>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Resposta de geocoding (primeiros 200 chars): {Content}",
                    jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent);

                var geocodingResponse = JsonSerializer.Deserialize<GeocodingResponse>(jsonContent, _jsonOptions);

                return geocodingResponse?.Results ?? new List<LocationResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar coordenadas para cidade: {City}", cityName);
                return new List<LocationResult>();
            }
        }

        /// <inheritdoc />
        public async Task<OpenMeteoResponse?> GetHistoricalWeatherAsync(
            double latitude,
            double longitude,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Buscando dados históricos para {Lat}, {Lon} de {Start} a {End}",
                    latitude, longitude, startDate.Date, endDate.Date);

                var url = $"/v1/archive?" +
                          $"latitude={latitude:F4}&longitude={longitude:F4}" +
                          $"&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}" +
                          $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,wind_speed_10m_max" +
                          $"&timezone=auto";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Erro na API Open-Meteo Historical: {StatusCode} - {ReasonPhrase} - Content: {Content}",
                        response.StatusCode, response.ReasonPhrase, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<OpenMeteoResponse>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados históricos");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsApiHealthyAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Fazer uma requisição simples para verificar se a API está funcionando
                var response = await GetWeatherByCoordinatesAsync(-23.5505, -46.6333, 1, cancellationToken);
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// Constrói a URL para buscar dados climáticos
        /// </summary>
        private string BuildWeatherUrl(double latitude, double longitude, int forecastDays)
        {
            // URL relativa para usar com BaseAddress
            return $"/v1/forecast?" +
                   $"latitude={latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}" +
                   $"&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover,pressure_msl,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                   $"&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,precipitation,rain,weather_code,pressure_msl,cloud_cover,wind_speed_10m,wind_direction_10m" +
                   $"&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,precipitation_sum,rain_sum,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,wind_direction_10m_dominant" +
                   $"&timezone=auto" +
                   $"&forecast_days={forecastDays}";
        }

        /// <summary>
        /// Mapeia a resposta da Open-Meteo para nosso DTO
        /// </summary>
        private WeatherResponse MapToWeatherResponse(OpenMeteoResponse openMeteoResponse)
        {
            var response = new WeatherResponse
            {
                Latitude = openMeteoResponse.Latitude,
                Longitude = openMeteoResponse.Longitude,
                Timezone = openMeteoResponse.Timezone,
                RetrievedAt = DateTime.UtcNow
            };

            // Mapear dados atuais
            if (openMeteoResponse.Current != null)
            {
                response.Current = new CurrentWeatherDto
                {
                    Time = DateTime.TryParse(openMeteoResponse.Current.Time, out var currentTime) ? currentTime : DateTime.UtcNow,
                    Temperature = openMeteoResponse.Current.Temperature,
                    FeelsLike = openMeteoResponse.Current.ApparentTemperature,
                    Humidity = openMeteoResponse.Current.RelativeHumidity,
                    Pressure = openMeteoResponse.Current.Pressure,
                    WindSpeed = openMeteoResponse.Current.WindSpeed,
                    WindDirection = openMeteoResponse.Current.WindDirection,
                    CloudCover = openMeteoResponse.Current.CloudCover,
                    Precipitation = openMeteoResponse.Current.Precipitation,
                    WeatherDescription = WeatherCodes.GetDescription(openMeteoResponse.Current.WeatherCode),
                    IsDay = openMeteoResponse.Current.IsDay == 1
                };
            }

            // Mapear previsão diária
            if (openMeteoResponse.Daily != null && openMeteoResponse.Daily.Time.Any())
            {
                for (int i = 0; i < openMeteoResponse.Daily.Time.Count; i++)
                {
                    if (DateTime.TryParse(openMeteoResponse.Daily.Time[i], out var date))
                    {
                        response.DailyForecast.Add(new DailyWeatherDto
                        {
                            Date = date,
                            TemperatureMax = openMeteoResponse.Daily.TemperatureMax.ElementAtOrDefault(i),
                            TemperatureMin = openMeteoResponse.Daily.TemperatureMin.ElementAtOrDefault(i),
                            PrecipitationSum = openMeteoResponse.Daily.PrecipitationSum.ElementAtOrDefault(i),
                            PrecipitationProbability = openMeteoResponse.Daily.PrecipitationProbabilityMax.ElementAtOrDefault(i),
                            WindSpeedMax = openMeteoResponse.Daily.WindSpeedMax.ElementAtOrDefault(i),
                            WeatherDescription = WeatherCodes.GetDescription(openMeteoResponse.Daily.WeatherCode.ElementAtOrDefault(i))
                        });
                    }
                }
            }

            return response;
        }

        // Implementar IDisposable para limpar recursos
        public void Dispose()
        {
            _geocodingClient?.Dispose();
        }

        #endregion
    }
}