using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherMap.Models
{
    /// <summary>
    /// Previsões diárias associadas a um histórico climático
    /// </summary>
    public class DailyForecast
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WeatherHistoryId { get; set; }

        [Required]
        public DateTime ForecastDate { get; set; }

        public double TemperatureMax { get; set; }
        public double TemperatureMin { get; set; }
        public double PrecipitationSum { get; set; }
        public int PrecipitationProbability { get; set; }
        public double WindSpeedMax { get; set; }

        [MaxLength(100)]
        public string WeatherDescription { get; set; } = string.Empty;

        // Relacionamento N:1 com WeatherHistory
        [ForeignKey("WeatherHistoryId")]
        public virtual WeatherHistory? WeatherHistory { get; set; }
    }

    // Versão anterior, mantida para referência
    // public class DailyForecast
    // {
    //     public int Id { get; set; }
    //     public DateTime Date { get; set; }

    //     public double? TemperatureMax { get; set; }
    //     public double? TemperatureMin { get; set; }
    //     public double? PrecipitationSum { get; set; }
    //     public int? PrecipitationProbability { get; set; }
    //     public double? WindSpeedMax { get; set; }

    //     public int? WeatherCode { get; set; }
    //     public string? WeatherDescription { get; set; } // opcional, alinhado com seu DTO

    //     public int WeatherHistoryId { get; set; }
    //     public WeatherHistory? WeatherHistory { get; set; }
    // }

}