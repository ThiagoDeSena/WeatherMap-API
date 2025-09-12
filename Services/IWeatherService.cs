using WeatherMap.DTOs;

namespace WeatherMap.Services
{
    // Interface do serviço
    /// <summary>
    /// Interface para serviços de dados climáticos
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Busca dados climáticos por coordenadas
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="forecastDays">Número de dias de previsão (padrão: 7)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados climáticos</returns>
        Task<WeatherResponse?> GetWeatherByCoordinatesAsync(
            double latitude,
            double longitude,
            int forecastDays = 7,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca dados climáticos por nome da cidade
        /// </summary>
        /// <param name="cityName">Nome da cidade</param>
        /// <param name="countryCode">Código do país (opcional)</param>
        /// <param name="forecastDays">Número de dias de previsão (padrão: 7)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados climáticos</returns>
        Task<WeatherResponse?> GetWeatherByCityAsync(
            string cityName,
            string? countryCode = null,
            int forecastDays = 7,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca coordenadas por nome da cidade (Geocoding)
        /// </summary>
        /// <param name="cityName">Nome da cidade</param>
        /// <param name="countryCode">Código do país (opcional)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de localizações encontradas</returns>
        Task<List<LocationResult>> GetLocationsByNameAsync(
            string cityName,
            string? countryCode = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca dados históricos do clima
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados históricos</returns>
        Task<OpenMeteoResponse?> GetHistoricalWeatherAsync(
            double latitude,
            double longitude,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se a API está funcionando
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se a API estiver funcionando</returns>
        Task<bool> IsApiHealthyAsync(CancellationToken cancellationToken = default);
    }
}