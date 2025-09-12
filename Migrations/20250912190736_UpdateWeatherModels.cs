using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherMap.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWeatherModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "PrecipitationSum",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "RawJson",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "TemperatureCurrent",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "TemperatureMax",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "TemperatureMin",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "WeatherCode",
                table: "DailyForecasts");

            migrationBuilder.RenameColumn(
                name: "ReferenceDate",
                table: "WeatherHistories",
                newName: "RetrievedAt");

            migrationBuilder.RenameColumn(
                name: "RecordedAt",
                table: "WeatherHistories",
                newName: "CurrentTime");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "DailyForecasts",
                newName: "ForecastDate");

            migrationBuilder.AlterColumn<string>(
                name: "Timezone",
                table: "WeatherHistories",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WeatherHistories",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CurrentCloudCover",
                table: "WeatherHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentFeelsLike",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHumidity",
                table: "WeatherHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CurrentIsDay",
                table: "WeatherHistories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "CurrentPrecipitation",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentPressure",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentTemperature",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CurrentWeatherDescription",
                table: "WeatherHistories",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "CurrentWindDirection",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentWindSpeed",
                table: "WeatherHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "WindSpeedMax",
                table: "DailyForecasts",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "DailyForecasts",
                keyColumn: "WeatherDescription",
                keyValue: null,
                column: "WeatherDescription",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherDescription",
                table: "DailyForecasts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "TemperatureMin",
                table: "DailyForecasts",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TemperatureMax",
                table: "DailyForecasts",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PrecipitationSum",
                table: "DailyForecasts",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PrecipitationProbability",
                table: "DailyForecasts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentCloudCover",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentFeelsLike",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentHumidity",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentIsDay",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentPrecipitation",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentPressure",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentTemperature",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentWeatherDescription",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentWindDirection",
                table: "WeatherHistories");

            migrationBuilder.DropColumn(
                name: "CurrentWindSpeed",
                table: "WeatherHistories");

            migrationBuilder.RenameColumn(
                name: "RetrievedAt",
                table: "WeatherHistories",
                newName: "ReferenceDate");

            migrationBuilder.RenameColumn(
                name: "CurrentTime",
                table: "WeatherHistories",
                newName: "RecordedAt");

            migrationBuilder.RenameColumn(
                name: "ForecastDate",
                table: "DailyForecasts",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Timezone",
                table: "WeatherHistories",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "WeatherHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PrecipitationSum",
                table: "WeatherHistories",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawJson",
                table: "WeatherHistories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "TemperatureCurrent",
                table: "WeatherHistories",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureMax",
                table: "WeatherHistories",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureMin",
                table: "WeatherHistories",
                type: "double",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "WindSpeedMax",
                table: "DailyForecasts",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherDescription",
                table: "DailyForecasts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "TemperatureMin",
                table: "DailyForecasts",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "TemperatureMax",
                table: "DailyForecasts",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "PrecipitationSum",
                table: "DailyForecasts",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "PrecipitationProbability",
                table: "DailyForecasts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WeatherCode",
                table: "DailyForecasts",
                type: "int",
                nullable: true);
        }
    }
}
