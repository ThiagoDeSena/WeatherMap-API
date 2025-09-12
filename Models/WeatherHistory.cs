using System.ComponentModel.DataAnnotations;

namespace WeatherMap.Models
{
    public class WeatherHistory
    {
        public int Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime ReferenceDate { get; set; }

        public double? TemperatureCurrent { get; set; }
        public double? TemperatureMax { get; set; }
        public double? TemperatureMin { get; set; }
        public double? PrecipitationSum { get; set; }
        public int? Humidity { get; set; } // pode vir do CurrentWeatherDto

        public string Timezone { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public string? RawJson { get; set; }

        public List<DailyForecast> DailyForecasts { get; set; } = new();
    }

}