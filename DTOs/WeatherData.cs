namespace WeatherMap.DTOs
{
    //Dados climáticos processados
    /// <summary>
    /// Request para buscar dados climáticos
    /// </summary>
    public class WeatherData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CityName { get; set; }
        public string? CountryCode { get; set; }
        public int ForecastDays { get; set; } = 7;
        public bool IncludeHourly { get; set; } = true;
        public bool IncludeCurrent { get; set; } = true;
        public string Timezone { get; set; } = "auto";
    }

    /// <summary>
    /// DTO simplificado para resposta da nossa API
    /// </summary>
    public class WeatherResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public CurrentWeatherDto Current { get; set; } = new();
        public List<DailyWeatherDto> DailyForecast { get; set; } = new();
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Dados atuais simplificados
    /// </summary>
    public class CurrentWeatherDto
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public int CloudCover { get; set; }
        public double Precipitation { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public bool IsDay { get; set; }
    }

    /// <summary>
    /// Previsão diária simplificada
    /// </summary>
    public class DailyWeatherDto
    {
        public DateTime Date { get; set; }
        public double TemperatureMax { get; set; }
        public double TemperatureMin { get; set; }
        public double PrecipitationSum { get; set; }
        public int PrecipitationProbability { get; set; }
        public double WindSpeedMax { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para geocoding (buscar coordenadas por nome da cidade)
    /// </summary>
    public class GeocodingResponse
    {
        public List<LocationResult> Results { get; set; } = new();
    }

    public class LocationResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string? Feature_code { get; set; }
        public string? Country_code { get; set; }
        public string? Country { get; set; }
        public string? Timezone { get; set; }
        public int Population { get; set; }
        public List<string>? Postcodes { get; set; }
        public int Country_id { get; set; }
        public string? Admin1 { get; set; }
        public string? Admin2 { get; set; }
        public string? Admin3 { get; set; }
        public string? Admin4 { get; set; }
        public int Admin1_id { get; set; }
        public int Admin2_id { get; set; }
        public int Admin3_id { get; set; }
        public int Admin4_id { get; set; }
    }
}