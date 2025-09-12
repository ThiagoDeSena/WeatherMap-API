using System.ComponentModel.DataAnnotations;

namespace WeatherMap.Models
{
    /// <summary>
    /// Histórico de consultas climáticas salvas no banco
    /// </summary>
    public class WeatherHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [MaxLength(200)]
        public string LocationName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Timezone { get; set; } = string.Empty;

        // Dados climáticos atuais no momento da consulta
        public DateTime CurrentTime { get; set; }
        public double CurrentTemperature { get; set; }
        public double CurrentFeelsLike { get; set; }
        public int CurrentHumidity { get; set; }
        public double CurrentPressure { get; set; }
        public double CurrentWindSpeed { get; set; }
        public double CurrentWindDirection { get; set; }
        public int CurrentCloudCover { get; set; }
        public double CurrentPrecipitation { get; set; }

        [MaxLength(100)]
        public string CurrentWeatherDescription { get; set; } = string.Empty;
        public bool CurrentIsDay { get; set; }

        // Metadados
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamento 1:N com previsões diárias
        public virtual ICollection<DailyForecast> DailyForecasts { get; set; } = new List<DailyForecast>();
    }

}