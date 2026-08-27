using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WeatherDashboard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    OpenWeatherCityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CollectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemperatureC = table.Column<double>(type: "float", nullable: false),
                    FeelsLikeC = table.Column<double>(type: "float", nullable: false),
                    TempMinC = table.Column<double>(type: "float", nullable: false),
                    TempMaxC = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    PressureHpa = table.Column<double>(type: "float", nullable: false),
                    WindSpeedMs = table.Column<double>(type: "float", nullable: false),
                    WeatherDescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WeatherIcon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherReadings_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Latitude", "Longitude", "Name", "OpenWeatherCityId", "State" },
                values: new object[,]
                {
                    { 1, -9.97499, -67.824299999999994, "Rio Branco", null, "AC" },
                    { 2, -9.6659900000000007, -35.734999999999999, "Maceió", null, "AL" },
                    { 3, 0.038899999999999997, -51.066400000000002, "Macapá", null, "AP" },
                    { 4, -3.1019399999999999, -60.024999999999999, "Manaus", null, "AM" },
                    { 5, -12.9711, -38.510800000000003, "Salvador", null, "BA" },
                    { 6, -3.7172200000000002, -38.543100000000003, "Fortaleza", null, "CE" },
                    { 7, -15.7797, -47.929699999999997, "Brasília", null, "DF" },
                    { 8, -20.319400000000002, -40.337800000000001, "Vitória", null, "ES" },
                    { 9, -16.686900000000001, -49.264800000000001, "Goiânia", null, "GO" },
                    { 10, -2.5297200000000002, -44.302799999999998, "São Luís", null, "MA" },
                    { 11, -15.5961, -56.096699999999998, "Cuiabá", null, "MT" },
                    { 12, -20.442799999999998, -54.6464, "Campo Grande", null, "MS" },
                    { 13, -19.9208, -43.937800000000003, "Belo Horizonte", null, "MG" },
                    { 14, -1.45583, -48.503900000000002, "Belém", null, "PA" },
                    { 15, -7.1150000000000002, -34.863100000000003, "João Pessoa", null, "PB" },
                    { 16, -25.427800000000001, -49.273099999999999, "Curitiba", null, "PR" },
                    { 17, -8.0538900000000009, -34.881100000000004, "Recife", null, "PE" },
                    { 18, -5.0891700000000002, -42.801900000000003, "Teresina", null, "PI" },
                    { 19, -22.9068, -43.172899999999998, "Rio de Janeiro", null, "RJ" },
                    { 20, -5.7949999999999999, -35.209400000000002, "Natal", null, "RN" },
                    { 21, -30.033100000000001, -51.229999999999997, "Porto Alegre", null, "RS" },
                    { 22, -8.7619399999999992, -63.9039, "Porto Velho", null, "RO" },
                    { 23, 2.8197199999999998, -60.673299999999998, "Boa Vista", null, "RR" },
                    { 24, -27.595400000000001, -48.548000000000002, "Florianópolis", null, "SC" },
                    { 25, -23.547499999999999, -46.636099999999999, "São Paulo", null, "SP" },
                    { 26, -10.911099999999999, -37.0717, "Aracaju", null, "SE" },
                    { 27, -10.2128, -48.360300000000002, "Palmas", null, "TO" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_State",
                table: "Cities",
                columns: new[] { "Name", "State" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeatherReadings_CityId_CollectedAtUtc",
                table: "WeatherReadings",
                columns: new[] { "CityId", "CollectedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherReadings");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
