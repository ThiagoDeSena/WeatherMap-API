namespace WeatherMap.Models
{
    public class DailyForecast
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public double? TemperatureMax { get; set; }
        public double? TemperatureMin { get; set; }
        public double? PrecipitationSum { get; set; }
        public int? PrecipitationProbability { get; set; }
        public double? WindSpeedMax { get; set; }

        public int? WeatherCode { get; set; }
        public string? WeatherDescription { get; set; } // opcional, alinhado com seu DTO

        public int WeatherHistoryId { get; set; }
        public WeatherHistory? WeatherHistory { get; set; }
    }

}