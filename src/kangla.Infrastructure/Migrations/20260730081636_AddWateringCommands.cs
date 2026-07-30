using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWateringCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterNow",
                table: "WateringDevices");

            migrationBuilder.CreateTable(
                name: "WateringCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WateringDeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcknowledgedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FinishedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WateringEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WateringCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WateringCommands_WateringDevices_WateringDeviceId",
                        column: x => x.WateringDeviceId,
                        principalTable: "WateringDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WateringCommands_WateringEvents_WateringEventId",
                        column: x => x.WateringEventId,
                        principalTable: "WateringEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WateringCommands_Status",
                table: "WateringCommands",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WateringCommands_WateringDeviceId",
                table: "WateringCommands",
                column: "WateringDeviceId",
                unique: true,
                filter: "\"Status\" IN ('Pending', 'Acknowledged')");

            migrationBuilder.CreateIndex(
                name: "IX_WateringCommands_WateringDeviceId_Status",
                table: "WateringCommands",
                columns: new[] { "WateringDeviceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WateringCommands_WateringEventId",
                table: "WateringCommands",
                column: "WateringEventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WateringCommands");

            migrationBuilder.AddColumn<bool>(
                name: "WaterNow",
                table: "WateringDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
