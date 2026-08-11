using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantSoilMoisturePercentages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumSoilHumidity",
                table: "WateringDevices");

            migrationBuilder.AddColumn<int>(
                name: "DesiredSoilMoisturePercentage",
                table: "Plants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoilMoisturePercentage",
                table: "HumidityMeasurements",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredSoilMoisturePercentage",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "SoilMoisturePercentage",
                table: "HumidityMeasurements");

            migrationBuilder.AddColumn<int>(
                name: "MinimumSoilHumidity",
                table: "WateringDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
