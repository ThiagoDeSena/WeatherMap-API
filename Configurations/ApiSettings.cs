namespace WeatherMap.Configurations
{
    // Configurações da API
    /// <summary>
    /// Configurações para APIs externas
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// URL base da API Open-Meteo
        /// </summary>
        public string OpenMeteoBaseUrl { get; set; } = "https://api.open-meteo.com";

        /// <summary>
        /// URL para geocoding (buscar coordenadas por nome da cidade)
        /// </summary>
        public string GeocodingBaseUrl { get; set; } = "https://geocoding-api.open-meteo.com";

        /// <summary>
        /// Timeout para requests HTTP (em segundos)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Número máximo de tentativas em caso de falha
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// User-Agent para identificar sua aplicação
        /// </summary>
        public string UserAgent { get; set; } = "WeatherMapAPI/1.0";
    }

    /// <summary>
    /// Códigos de clima do Open-Meteo para descrições amigáveis
    /// </summary>
    public static class WeatherCodes
    {
        public static readonly Dictionary<int, string> Descriptions = new()
        {
            { 0, "Céu limpo" },
            { 1, "Principalmente limpo" },
            { 2, "Parcialmente nublado" },
            { 3, "Nublado" },
            { 45, "Névoa" },
            { 48, "Névoa com geada" },
            { 51, "Garoa leve" },
            { 53, "Garoa moderada" },
            { 55, "Garoa intensa" },
            { 56, "Garoa gelada leve" },
            { 57, "Garoa gelada intensa" },
            { 61, "Chuva leve" },
            { 63, "Chuva moderada" },
            { 65, "Chuva intensa" },
            { 66, "Chuva gelada leve" },
            { 67, "Chuva gelada intensa" },
            { 71, "Neve leve" },
            { 73, "Neve moderada" },
            { 75, "Neve intensa" },
            { 77, "Granizo" },
            { 80, "Pancadas de chuva leves" },
            { 81, "Pancadas de chuva moderadas" },
            { 82, "Pancadas de chuva violentas" },
            { 85, "Pancadas de neve leves" },
            { 86, "Pancadas de neve intensas" },
            { 95, "Tempestade" },
            { 96, "Tempestade com granizo leve" },
            { 99, "Tempestade com granizo intenso" }
        };

        public static string GetDescription(int code)
        {
            return Descriptions.TryGetValue(code, out var description) ? description : "Desconhecido";
        }
    }
}