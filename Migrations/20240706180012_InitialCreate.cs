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
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    WaterNow = table.Column<bool>(type: "INTEGER", nullable: false),
                    WateringIntervalSetting = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringDurationSetting = table.Column<int>(type: "INTEGER", nullable: false)
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
                    SoilHumidity = table.Column<double>(type: "REAL", nullable: false),
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
                columns: new[] { "Id", "Active", "Deleted", "Description", "Location", "Name", "Notes", "WaterNow", "WateringDurationSetting", "WateringIntervalSetting" },
                values: new object[,]
                {
                    { 1, true, false, "My device description", "My location", "My device", "My notes", false, 0, 0 },
                    { 2, true, false, "My device description 2", "My location 2", "My device 2", "My notes 2", false, 0, 0 },
                    { 3, true, false, "My device description 3", "My location 3", "My device 3", "My notes 3", false, 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "HumidityMeasurements",
                columns: new[] { "Id", "DateTime", "SoilHumidity", "WateringDeviceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 45.200000000000003, 1 },
                    { 2, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 44.799999999999997, 1 },
                    { 3, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 44.0, 1 },
                    { 4, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 43.5, 1 },
                    { 5, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 43.0, 1 },
                    { 6, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 52.100000000000001, 2 },
                    { 7, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 51.799999999999997, 2 },
                    { 8, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 51.399999999999999, 2 },
                    { 9, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 51.0, 2 },
                    { 10, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 50.600000000000001, 2 },
                    { 11, new DateTime(2024, 7, 6, 6, 0, 0, 0, DateTimeKind.Utc), 60.5, 3 },
                    { 12, new DateTime(2024, 7, 6, 7, 0, 0, 0, DateTimeKind.Utc), 60.0, 3 },
                    { 13, new DateTime(2024, 7, 6, 8, 0, 0, 0, DateTimeKind.Utc), 59.600000000000001, 3 },
                    { 14, new DateTime(2024, 7, 6, 9, 0, 0, 0, DateTimeKind.Utc), 59.200000000000003, 3 },
                    { 15, new DateTime(2024, 7, 6, 10, 0, 0, 0, DateTimeKind.Utc), 58.899999999999999, 3 }
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
