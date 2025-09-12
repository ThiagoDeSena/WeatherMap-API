using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherMap.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WeatherHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LocationName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<double>(type: "double", nullable: false),
                    Longitude = table.Column<double>(type: "double", nullable: false),
                    ReferenceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TemperatureCurrent = table.Column<double>(type: "double", nullable: true),
                    TemperatureMax = table.Column<double>(type: "double", nullable: true),
                    TemperatureMin = table.Column<double>(type: "double", nullable: true),
                    PrecipitationSum = table.Column<double>(type: "double", nullable: true),
                    Humidity = table.Column<int>(type: "int", nullable: true),
                    Timezone = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RawJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherHistories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DailyForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double", nullable: true),
                    TemperatureMin = table.Column<double>(type: "double", nullable: true),
                    PrecipitationSum = table.Column<double>(type: "double", nullable: true),
                    PrecipitationProbability = table.Column<int>(type: "int", nullable: true),
                    WindSpeedMax = table.Column<double>(type: "double", nullable: true),
                    WeatherCode = table.Column<int>(type: "int", nullable: true),
                    WeatherDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeatherHistoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyForecasts_WeatherHistories_WeatherHistoryId",
                        column: x => x.WeatherHistoryId,
                        principalTable: "WeatherHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DailyForecasts_WeatherHistoryId",
                table: "DailyForecasts",
                column: "WeatherHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyForecasts");

            migrationBuilder.DropTable(
                name: "WeatherHistories");
        }
    }
}
