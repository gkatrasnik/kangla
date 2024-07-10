using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kangla_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WateringDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    WaterNow = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastWatered = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MinimumSoilHumidity = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringIntervalSetting = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringDurationSetting = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceToken = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WateringDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HumidityMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SoilHumidity = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HumidityMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HumidityMeasurements_WateringDevices_WateringDeviceId",
                        column: x => x.WateringDeviceId,
                        principalTable: "WateringDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WateringEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WateringEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WateringEvents_WateringDevices_WateringDeviceId",
                        column: x => x.WateringDeviceId,
                        principalTable: "WateringDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "WateringDevices",
                columns: new[] { "Id", "Active", "Deleted", "Description", "DeviceToken", "LastWatered", "Location", "MinimumSoilHumidity", "Name", "Notes", "WaterNow", "WateringDurationSetting", "WateringIntervalSetting" },
                values: new object[,]
                {
                    { 1, true, false, "First watering device", "abcdefghi0", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Garden", 400, "Device 1", "Needs regular maintenance", false, 5, 30 },
                    { 2, true, false, "Second watering device", "abcdefghi1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Greenhouse", 400, "Device 2", "Check humidity levels", true, 4, 20 },
                    { 3, true, false, "Third watering device", "abcdefghi2", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Front Yard", 400, "Device 3", "Monitor water usage", false, 3, 25 }
                });

            migrationBuilder.InsertData(
                table: "HumidityMeasurements",
                columns: new[] { "Id", "DateTime", "SoilHumidity", "WateringDeviceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 657, 1 },
                    { 2, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 349, 1 },
                    { 3, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 472, 1 },
                    { 4, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 512, 1 },
                    { 5, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 276, 1 },
                    { 6, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 362, 2 },
                    { 7, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 693, 2 },
                    { 8, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 448, 2 },
                    { 9, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 275, 2 },
                    { 10, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 508, 2 },
                    { 11, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 731, 3 },
                    { 12, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 371, 3 },
                    { 13, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 689, 3 },
                    { 14, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 381, 3 },
                    { 15, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 585, 3 }
                });

            migrationBuilder.InsertData(
                table: "WateringEvents",
                columns: new[] { "Id", "End", "Start", "WateringDeviceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 6, 8, 5, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 7, 7, 8, 5, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 7, 8, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, new DateTime(2024, 7, 6, 9, 4, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 4, new DateTime(2024, 7, 7, 9, 4, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 7, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 5, new DateTime(2024, 7, 6, 10, 3, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 6, new DateTime(2024, 7, 7, 10, 3, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 7, 10, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HumidityMeasurements_WateringDeviceId",
                table: "HumidityMeasurements",
                column: "WateringDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_WateringEvents_WateringDeviceId",
                table: "WateringEvents",
                column: "WateringDeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HumidityMeasurements");

            migrationBuilder.DropTable(
                name: "WateringEvents");

            migrationBuilder.DropTable(
                name: "WateringDevices");
        }
    }
}
