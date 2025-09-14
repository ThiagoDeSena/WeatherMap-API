// using Microsoft.EntityFrameworkCore;
// using WeatherMap.Data;
// using WeatherMap.DTOs;
// using WeatherMap.Models;

// namespace WeatherMap.Services
// {
//     public interface IWeatherDatabaseService
//     {
//         Task<WeatherHistory> SaveWeatherDataAsync(WeatherResponse weatherResponse);
//         Task<List<WeatherHistory>> GetWeatherHistoryAsync(int limit = 50);
//         Task<List<WeatherHistory>> GetWeatherByLocationAsync(string locationName);
//         Task<WeatherHistory?> GetWeatherByIdAsync(int id);
//         Task<List<WeatherHistory>> GetWeatherByCoordinatesAsync(double latitude, double longitude, double tolerance = 0.01);
//         Task<List<DailyForecast>> GetForecastsForLocationAsync(string locationName, DateTime? startDate = null, DateTime? endDate = null);
//     }

//     public class WeatherDatabaseService : IWeatherDatabaseService
//     {
//         private readonly AppDbContext _context;
//         private readonly ILogger<WeatherDatabaseService> _logger;

//         public WeatherDatabaseService(AppDbContext context, ILogger<WeatherDatabaseService> logger)
//         {
//             _context = context;
//             _logger = logger;
//         }

//         public async Task<WeatherHistory> SaveWeatherDataAsync(WeatherResponse weatherResponse)
//         {
//             try
//             {
//                 _logger.LogInformation("Salvando dados climáticos para {Location}", weatherResponse.Location);

//                 var weatherHistory = new WeatherHistory
//                 {
//                     Latitude = weatherResponse.Latitude,
//                     Longitude = weatherResponse.Longitude,
//                     LocationName = weatherResponse.Location,
//                     Timezone = weatherResponse.Timezone,

//                     // Dados atuais
//                     CurrentTime = weatherResponse.Current.Time,
//                     CurrentTemperature = weatherResponse.Current.Temperature,
//                     CurrentFeelsLike = weatherResponse.Current.FeelsLike,
//                     CurrentHumidity = weatherResponse.Current.Humidity,
//                     CurrentPressure = weatherResponse.Current.Pressure,
//                     CurrentWindSpeed = weatherResponse.Current.WindSpeed,
//                     CurrentWindDirection = weatherResponse.Current.WindDirection,
//                     CurrentCloudCover = weatherResponse.Current.CloudCover,
//                     CurrentPrecipitation = weatherResponse.Current.Precipitation,
//                     CurrentWeatherDescription = weatherResponse.Current.WeatherDescription,
//                     CurrentIsDay = weatherResponse.Current.IsDay,

//                     RetrievedAt = weatherResponse.RetrievedAt,
//                     CreatedAt = DateTime.UtcNow
//                 };

//                 // Adicionar previsões diárias
//                 foreach (var daily in weatherResponse.DailyForecast)
//                 {
//                     weatherHistory.DailyForecasts.Add(new DailyForecast
//                     {
//                         ForecastDate = daily.Date,
//                         TemperatureMax = daily.TemperatureMax,
//                         TemperatureMin = daily.TemperatureMin,
//                         PrecipitationSum = daily.PrecipitationSum,
//                         PrecipitationProbability = daily.PrecipitationProbability,
//                         WindSpeedMax = daily.WindSpeedMax,
//                         WeatherDescription = daily.WeatherDescription
//                     });
//                 }

//                 _context.WeatherHistories.Add(weatherHistory);
//                 await _context.SaveChangesAsync();

//                 _logger.LogInformation("Dados climáticos salvos com ID {Id} para {Location}", weatherHistory.Id, weatherResponse.Location);

//                 return weatherHistory;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Erro ao salvar dados climáticos para {Location}", weatherResponse.Location);
//                 throw;
//             }
//         }

//         public async Task<List<WeatherHistory>> GetWeatherHistoryAsync(int limit = 50)
//         {
//             return await _context.WeatherHistories
//                 .Include(w => w.DailyForecasts)
//                 .OrderByDescending(w => w.CreatedAt)
//                 .Take(limit)
//                 .ToListAsync();
//         }

//         public async Task<List<WeatherHistory>> GetWeatherByLocationAsync(string locationName)
//         {
//             return await _context.WeatherHistories
//                 .Include(w => w.DailyForecasts)
//                 .Where(w => w.LocationName.Contains(locationName))
//                 .OrderByDescending(w => w.CreatedAt)
//                 .ToListAsync();
//         }

//         public async Task<WeatherHistory?> GetWeatherByIdAsync(int id)
//         {
//             return await _context.WeatherHistories
//                 .Include(w => w.DailyForecasts)
//                 .FirstOrDefaultAsync(w => w.Id == id);
//         }

//         public async Task<List<WeatherHistory>> GetWeatherByCoordinatesAsync(double latitude, double longitude, double tolerance = 0.01)
//         {
//             return await _context.WeatherHistories
//                 .Include(w => w.DailyForecasts)
//                 .Where(w => Math.Abs(w.Latitude - latitude) <= tolerance &&
//                            Math.Abs(w.Longitude - longitude) <= tolerance)
//                 .OrderByDescending(w => w.CreatedAt)
//                 .ToListAsync();
//         }

//         public async Task<List<DailyForecast>> GetForecastsForLocationAsync(string locationName, DateTime? startDate = null, DateTime? endDate = null)
//         {
//             var query = _context.DailyForecasts
//                 .Include(d => d.WeatherHistory)
//                 .Where(d => d.WeatherHistory!.LocationName.Contains(locationName));

//             if (startDate.HasValue)
//                 query = query.Where(d => d.ForecastDate >= startDate.Value);

//             if (endDate.HasValue)
//                 query = query.Where(d => d.ForecastDate <= endDate.Value);

//             return await query
//                 .OrderBy(d => d.ForecastDate)
//                 .ToListAsync();
//         }
//     }
// }

using Microsoft.EntityFrameworkCore;
using WeatherMap.Data;
using WeatherMap.DTOs;
using WeatherMap.Models;

namespace WeatherMap.Services
{
    public interface IWeatherDatabaseService
    {
        // Operações básicas CRUD
        Task<WeatherHistory> SaveWeatherDataAsync(WeatherResponse weatherResponse);
        Task<List<WeatherHistory>> GetWeatherHistoryAsync(int limit = 50);
        Task<List<WeatherHistory>> GetWeatherByLocationAsync(string locationName);
        Task<WeatherHistory?> GetWeatherByIdAsync(int id);
        Task<List<WeatherHistory>> GetWeatherByCoordinatesAsync(double latitude, double longitude, double tolerance = 0.01);
        Task<List<DailyForecast>> GetForecastsForLocationAsync(string locationName, DateTime? startDate = null, DateTime? endDate = null);

        // CRUD adicional
        Task<WeatherHistory?> UpdateWeatherHistoryAsync(int id, string newLocationName);
        Task<bool> DeleteWeatherHistoryAsync(int id);
        Task<bool> DeleteOldWeatherDataAsync(DateTime cutoffDate);

        // Consultas SQL brutas complexas
        Task<List<WeatherLocationStats>> GetLocationStatisticsRawAsync(int days = 30);
        Task<List<WeatherTrendData>> GetTemperatureTrendsRawAsync(string locationName, int days = 30);
        Task<List<WeatherComparisonData>> GetLocationComparisonRawAsync(List<string> locationNames);
        Task<DatabaseHealthInfo> GetDatabaseHealthRawAsync();
    }

    // DTOs para consultas complexas
    public class WeatherLocationStats
    {
        public string LocationName { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public double AvgTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
        public DateTime FirstRecord { get; set; }
        public DateTime LastRecord { get; set; }
    }

    public class WeatherTrendData
    {
        public DateTime Date { get; set; }
        public double AvgTemp { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public int RecordCount { get; set; }
    }

    public class WeatherComparisonData
    {
        public string LocationName { get; set; } = string.Empty;
        public double AvgCurrentTemp { get; set; }
        public double AvgHumidity { get; set; }
        public double AvgWindSpeed { get; set; }
        public int TotalDays { get; set; }
    }

    public class DatabaseHealthInfo
    {
        public int TotalWeatherRecords { get; set; }
        public int TotalForecastRecords { get; set; }
        public DateTime OldestRecord { get; set; }
        public DateTime NewestRecord { get; set; }
        public string MostQueriedLocation { get; set; } = string.Empty;
        public int MostQueriedLocationCount { get; set; }
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

        // CRUD Adicional
        public async Task<WeatherHistory?> UpdateWeatherHistoryAsync(int id, string newLocationName)
        {
            try
            {
                var weatherHistory = await _context.WeatherHistories.FindAsync(id);

                if (weatherHistory == null)
                    return null;

                weatherHistory.LocationName = newLocationName;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Localização atualizada para ID {Id}: {NewLocation}", id, newLocationName);
                return weatherHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar localização para ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteWeatherHistoryAsync(int id)
        {
            try
            {
                var weatherHistory = await _context.WeatherHistories.FindAsync(id);

                if (weatherHistory == null)
                    return false;

                _context.WeatherHistories.Remove(weatherHistory);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Registro climático deletado: ID {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar registro climático ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteOldWeatherDataAsync(DateTime cutoffDate)
        {
            try
            {
                var oldRecords = await _context.WeatherHistories
                    .Where(w => w.CreatedAt < cutoffDate)
                    .ToListAsync();

                if (!oldRecords.Any())
                    return false;

                _context.WeatherHistories.RemoveRange(oldRecords);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deletados {Count} registros climáticos anteriores a {Date}", oldRecords.Count, cutoffDate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar registros antigos anteriores a {Date}", cutoffDate);
                throw;
            }
        }

        // Consultas SQL brutas complexas
        public async Task<List<WeatherLocationStats>> GetLocationStatisticsRawAsync(int days = 30)
        {
            try
            {
                _logger.LogInformation("Executando consulta SQL bruta para estatísticas de localização ({Days} dias)", days);

                var cutoffDate = DateTime.UtcNow.AddDays(-days);

                var sql = @"
                    SELECT 
                        LocationName,
                        COUNT(*) as TotalRecords,
                        ROUND(AVG(CurrentTemperature), 2) as AvgTemperature,
                        ROUND(MAX(CurrentTemperature), 2) as MaxTemperature,
                        ROUND(MIN(CurrentTemperature), 2) as MinTemperature,
                        MIN(CreatedAt) as FirstRecord,
                        MAX(CreatedAt) as LastRecord
                    FROM WeatherHistories 
                    WHERE CreatedAt >= {0}
                    GROUP BY LocationName 
                    ORDER BY TotalRecords DESC";

                return await _context.WeatherLocationStats
                    .FromSqlRaw(sql, cutoffDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de estatísticas");
                throw;
            }
        }

        public async Task<List<WeatherTrendData>> GetTemperatureTrendsRawAsync(string locationName, int days = 30)
        {
            try
            {
                _logger.LogInformation("Executando consulta SQL bruta para tendências de temperatura - {Location}", locationName);

                var cutoffDate = DateTime.UtcNow.AddDays(-days);

                var sql = @"
                    SELECT 
                        DATE(CreatedAt) as Date,
                        ROUND(AVG(CurrentTemperature), 2) as AvgTemp,
                        ROUND(MAX(CurrentTemperature), 2) as MaxTemp,
                        ROUND(MIN(CurrentTemperature), 2) as MinTemp,
                        COUNT(*) as RecordCount
                    FROM WeatherHistories 
                    WHERE LocationName LIKE {0} AND CreatedAt >= {1}
                    GROUP BY DATE(CreatedAt)
                    ORDER BY Date DESC";

                return await _context.WeatherTrendData
                    .FromSqlRaw(sql, $"%{locationName}%", cutoffDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de tendências para {Location}", locationName);
                throw;
            }
        }

        public async Task<List<WeatherComparisonData>> GetLocationComparisonRawAsync(List<string> locationNames)
        {
            try
            {
                _logger.LogInformation("Executando consulta SQL bruta para comparação entre localizações");

                if (!locationNames.Any())
                    return new List<WeatherComparisonData>();

                var placeholders = string.Join(",", locationNames.Select((_, i) => $"{{{i}}}"));
                var parameters = locationNames.Select(l => (object)$"%{l}%").ToArray();

                var sql = $@"
                    SELECT 
                        LocationName,
                        ROUND(AVG(CurrentTemperature), 2) as AvgCurrentTemp,
                        ROUND(AVG(CurrentHumidity), 2) as AvgHumidity,
                        ROUND(AVG(CurrentWindSpeed), 2) as AvgWindSpeed,
                        COUNT(DISTINCT DATE(CreatedAt)) as TotalDays
                    FROM WeatherHistories 
                    WHERE ({string.Join(" OR ", locationNames.Select((_, i) => $"LocationName LIKE {{{i}}}"))})
                    GROUP BY LocationName
                    ORDER BY AvgCurrentTemp DESC";

                return await _context.WeatherComparisonData
                    .FromSqlRaw(sql, parameters)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de comparação");
                throw;
            }
        }

        public async Task<DatabaseHealthInfo> GetDatabaseHealthRawAsync()
        {
            try
            {
                _logger.LogInformation("Executando consulta SQL bruta para saúde do banco");

                var sql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM WeatherHistories) as TotalWeatherRecords,
                        (SELECT COUNT(*) FROM DailyForecasts) as TotalForecastRecords,
                        (SELECT MIN(CreatedAt) FROM WeatherHistories) as OldestRecord,
                        (SELECT MAX(CreatedAt) FROM WeatherHistories) as NewestRecord,
                        (SELECT LocationName FROM WeatherHistories 
                         GROUP BY LocationName 
                         ORDER BY COUNT(*) DESC 
                         LIMIT 1) as MostQueriedLocation,
                        (SELECT COUNT(*) FROM WeatherHistories 
                         WHERE LocationName = (
                             SELECT LocationName FROM WeatherHistories 
                             GROUP BY LocationName 
                             ORDER BY COUNT(*) DESC 
                             LIMIT 1
                         )) as MostQueriedLocationCount";

                return await _context.DatabaseHealthInfo
                    .FromSqlRaw(sql)
                    .FirstOrDefaultAsync() ?? new DatabaseHealthInfo();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na consulta SQL bruta de saúde do banco");
                throw;
            }
        }
    }
}