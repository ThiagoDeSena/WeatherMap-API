using Microsoft.EntityFrameworkCore;
using WeatherMap.Data;
using WeatherMap.DTOs;
using WeatherMap.Models;

namespace WeatherMap.Services
{
    public interface IWeatherDatabaseService
    {
        Task<WeatherHistory> SaveWeatherDataAsync(WeatherResponse weatherResponse);
        Task<List<WeatherHistory>> GetWeatherHistoryAsync(int limit = 50);
        Task<List<WeatherHistory>> GetWeatherByLocationAsync(string locationName);
        Task<WeatherHistory?> GetWeatherByIdAsync(int id);
        Task<List<WeatherHistory>> GetWeatherByCoordinatesAsync(double latitude, double longitude, double tolerance = 0.01);
        Task<List<DailyForecast>> GetForecastsForLocationAsync(string locationName, DateTime? startDate = null, DateTime? endDate = null);
    }

    public class WeatherDatabaseService : IWeatherDatabaseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WeatherDatabaseService> _logger;

        public WeatherDatabaseService(AppDbContext context, ILogger<WeatherDatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WeatherHistory> SaveWeatherDataAsync(WeatherResponse weatherResponse)
        {
            try
            {
                _logger.LogInformation("Salvando dados climáticos para {Location}", weatherResponse.Location);

                var weatherHistory = new WeatherHistory
                {
                    Latitude = weatherResponse.Latitude,
                    Longitude = weatherResponse.Longitude,
                    LocationName = weatherResponse.Location,
                    Timezone = weatherResponse.Timezone,

                    // Dados atuais
                    CurrentTime = weatherResponse.Current.Time,
                    CurrentTemperature = weatherResponse.Current.Temperature,
                    CurrentFeelsLike = weatherResponse.Current.FeelsLike,
                    CurrentHumidity = weatherResponse.Current.Humidity,
                    CurrentPressure = weatherResponse.Current.Pressure,
                    CurrentWindSpeed = weatherResponse.Current.WindSpeed,
                    CurrentWindDirection = weatherResponse.Current.WindDirection,
                    CurrentCloudCover = weatherResponse.Current.CloudCover,
                    CurrentPrecipitation = weatherResponse.Current.Precipitation,
                    CurrentWeatherDescription = weatherResponse.Current.WeatherDescription,
                    CurrentIsDay = weatherResponse.Current.IsDay,

                    RetrievedAt = weatherResponse.RetrievedAt,
                    CreatedAt = DateTime.UtcNow
                };

                // Adicionar previsões diárias
                foreach (var daily in weatherResponse.DailyForecast)
                {
                    weatherHistory.DailyForecasts.Add(new DailyForecast
                    {
                        ForecastDate = daily.Date,
                        TemperatureMax = daily.TemperatureMax,
                        TemperatureMin = daily.TemperatureMin,
                        PrecipitationSum = daily.PrecipitationSum,
                        PrecipitationProbability = daily.PrecipitationProbability,
                        WindSpeedMax = daily.WindSpeedMax,
                        WeatherDescription = daily.WeatherDescription
                    });
                }

                _context.WeatherHistories.Add(weatherHistory);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Dados climáticos salvos com ID {Id} para {Location}", weatherHistory.Id, weatherResponse.Location);

                return weatherHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados climáticos para {Location}", weatherResponse.Location);
                throw;
            }
        }

        public async Task<List<WeatherHistory>> GetWeatherHistoryAsync(int limit = 50)
        {
            return await _context.WeatherHistories
                .Include(w => w.DailyForecasts)
                .OrderByDescending(w => w.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<WeatherHistory>> GetWeatherByLocationAsync(string locationName)
        {
            return await _context.WeatherHistories
                .Include(w => w.DailyForecasts)
                .Where(w => w.LocationName.Contains(locationName))
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<WeatherHistory?> GetWeatherByIdAsync(int id)
        {
            return await _context.WeatherHistories
                .Include(w => w.DailyForecasts)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<WeatherHistory>> GetWeatherByCoordinatesAsync(double latitude, double longitude, double tolerance = 0.01)
        {
            return await _context.WeatherHistories
                .Include(w => w.DailyForecasts)
                .Where(w => Math.Abs(w.Latitude - latitude) <= tolerance &&
                           Math.Abs(w.Longitude - longitude) <= tolerance)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<DailyForecast>> GetForecastsForLocationAsync(string locationName, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.DailyForecasts
                .Include(d => d.WeatherHistory)
                .Where(d => d.WeatherHistory!.LocationName.Contains(locationName));

            if (startDate.HasValue)
                query = query.Where(d => d.ForecastDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.ForecastDate <= endDate.Value);

            return await query
                .OrderBy(d => d.ForecastDate)
                .ToListAsync();
        }
    }
}