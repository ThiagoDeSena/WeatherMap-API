using System.Text.Json.Serialization;

namespace WeatherMap.DTOs
{
    // Resposta da API
    /// <summary>
    /// Resposta completa da API Open-Meteo
    /// </summary>
    public class OpenMeteoResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; } = string.Empty;

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyWeather? Hourly { get; set; }

        [JsonPropertyName("daily")]
        public DailyWeather? Daily { get; set; }
    }

    // <summary>
    /// Dados climáticos atuais
    /// </summary>
    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        [JsonPropertyName("precipitation")]
        public double Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public double Rain { get; set; }

        [JsonPropertyName("showers")]
        public double Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public double Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("cloud_cover")]
        public int CloudCover { get; set; }

        [JsonPropertyName("pressure_msl")]
        public double Pressure { get; set; }

        [JsonPropertyName("surface_pressure")]
        public double SurfacePressure { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public double WindDirection { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public double WindGusts { get; set; }
    }

    /// <summary>
    /// Dados climáticos por hora
    /// </summary>
    public class HourlyWeather
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature { get; set; } = new();

        [JsonPropertyName("relative_humidity_2m")]
        public List<int> RelativeHumidity { get; set; } = new();

        [JsonPropertyName("apparent_temperature")]
        public List<double> ApparentTemperature { get; set; } = new();

        [JsonPropertyName("precipitation_probability")]
        public List<int> PrecipitationProbability { get; set; } = new();

        [JsonPropertyName("precipitation")]
        public List<double> Precipitation { get; set; } = new();

        [JsonPropertyName("rain")]
        public List<double> Rain { get; set; } = new();

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new();

        [JsonPropertyName("pressure_msl")]
        public List<double> Pressure { get; set; } = new();

        [JsonPropertyName("cloud_cover")]
        public List<int> CloudCover { get; set; } = new();

        [JsonPropertyName("wind_speed_10m")]
        public List<double> WindSpeed { get; set; } = new();

        [JsonPropertyName("wind_direction_10m")]
        public List<double> WindDirection { get; set; } = new();
    }

    /// <summary>
    /// Dados climáticos diários
    /// </summary>
    public class DailyWeather
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new();

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperatureMax { get; set; } = new();

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperatureMin { get; set; } = new();

        [JsonPropertyName("apparent_temperature_max")]
        public List<double> ApparentTemperatureMax { get; set; } = new();

        [JsonPropertyName("apparent_temperature_min")]
        public List<double> ApparentTemperatureMin { get; set; } = new();

        [JsonPropertyName("precipitation_sum")]
        public List<double> PrecipitationSum { get; set; } = new();

        [JsonPropertyName("rain_sum")]
        public List<double> RainSum { get; set; } = new();

        [JsonPropertyName("precipitation_probability_max")]
        public List<int> PrecipitationProbabilityMax { get; set; } = new();

        [JsonPropertyName("wind_speed_10m_max")]
        public List<double> WindSpeedMax { get; set; } = new();

        [JsonPropertyName("wind_gusts_10m_max")]
        public List<double> WindGustsMax { get; set; } = new();

        [JsonPropertyName("wind_direction_10m_dominant")]
        public List<double> WindDirectionDominant { get; set; } = new();
    }
}