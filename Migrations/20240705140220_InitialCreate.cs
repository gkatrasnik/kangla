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
                name: "WateringEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    SoilHumidity = table.Column<double>(type: "REAL", nullable: false)
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
                table: "WateringEvents",
                columns: new[] { "Id", "DateTime", "Duration", "SoilHumidity", "WateringDeviceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 1, 8, 30, 0, 0, DateTimeKind.Unspecified), 10, 0.40000000000000002, 1 },
                    { 2, new DateTime(2024, 7, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 15, 0.34999999999999998, 2 },
                    { 3, new DateTime(2024, 7, 3, 7, 45, 0, 0, DateTimeKind.Unspecified), 12, 0.41999999999999998, 1 },
                    { 4, new DateTime(2024, 7, 4, 6, 50, 0, 0, DateTimeKind.Unspecified), 8, 0.38, 3 },
                    { 5, new DateTime(2024, 7, 5, 10, 15, 0, 0, DateTimeKind.Unspecified), 20, 0.33000000000000002, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WateringEvents_WateringDeviceId",
                table: "WateringEvents",
                column: "WateringDeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WateringEvents");

            migrationBuilder.DropTable(
                name: "WateringDevices");
        }
    }
}
