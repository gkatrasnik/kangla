using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RetainWateringDeviceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "WateringDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WateringDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "WateringDevices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WateringDevices");
        }
    }
}
