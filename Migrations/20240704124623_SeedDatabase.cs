using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kangla_backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WateringDevices",
                columns: new[] { "Id", "Active", "Deleted", "Description", "LastWatering", "Location", "Name", "Notes", "SoilHumidity", "WaterNow", "WateringDuration", "WateringInterval" },
                values: new object[,]
                {
                    { 1, true, false, "My device description", new DateTime(2024, 7, 4, 14, 46, 22, 852, DateTimeKind.Local).AddTicks(7021), "My location", "My device", "My notes", 0.5, false, 3, 300 },
                    { 2, true, false, "My device description 2", new DateTime(2024, 7, 4, 14, 46, 22, 852, DateTimeKind.Local).AddTicks(7073), "My location 2", "My device 2", "My notes 2", 0.5, false, 3, 300 },
                    { 3, true, false, "My device description 3", new DateTime(2024, 7, 4, 14, 46, 22, 852, DateTimeKind.Local).AddTicks(7076), "My location 3", "My device 3", "My notes 3", 0.5, false, 3, 300 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WateringDevices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WateringDevices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WateringDevices",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
