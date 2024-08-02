using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
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
                    MinimumSoilHumidity = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringIntervalSetting = table.Column<int>(type: "INTEGER", nullable: false),
                    WateringDurationSetting = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceToken = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
