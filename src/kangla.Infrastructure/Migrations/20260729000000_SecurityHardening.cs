using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    [DbContext(typeof(PlantsContext))]
    [Migration("20260729000000_SecurityHardening")]
    public partial class SecurityHardening : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE Images
                SET UserId = (
                    SELECT UserId
                    FROM Plants
                    WHERE Plants.ImageId = Images.Id
                    LIMIT 1
                )
                WHERE UserId = ''
                  AND EXISTS (
                      SELECT 1
                      FROM Plants
                      WHERE Plants.ImageId = Images.Id
                  );
                """);

            // Images not linked to a plant have no trustworthy owner and must not remain accessible.
            migrationBuilder.Sql("DELETE FROM Images WHERE UserId = '';");

            migrationBuilder.AddColumn<string>(
                name: "DeviceCredentialHash",
                table: "WateringDevices",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WateringDevices_DeviceCredentialHash",
                table: "WateringDevices",
                column: "DeviceCredentialHash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WateringDevices_DeviceCredentialHash",
                table: "WateringDevices");

            migrationBuilder.DropColumn(
                name: "DeviceCredentialHash",
                table: "WateringDevices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Images");
        }
    }
}
