using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeviceAccessKeyClaiming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WateringDevices_Plants_PlantId",
                table: "WateringDevices");

            migrationBuilder.DropIndex(
                name: "IX_WateringDevices_DeviceToken",
                table: "WateringDevices");

            migrationBuilder.DropColumn(
                name: "DeviceToken",
                table: "WateringDevices");

            migrationBuilder.RenameColumn(
                name: "DeviceCredentialHash",
                table: "WateringDevices",
                newName: "DeviceAccessKeyHash");

            migrationBuilder.RenameIndex(
                name: "IX_WateringDevices_DeviceCredentialHash",
                table: "WateringDevices",
                newName: "IX_WateringDevices_DeviceAccessKeyHash");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WateringDevices",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "WateringDevices",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_WateringDevices_Plants_PlantId",
                table: "WateringDevices",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WateringDevices_Plants_PlantId",
                table: "WateringDevices");

            migrationBuilder.RenameColumn(
                name: "DeviceAccessKeyHash",
                table: "WateringDevices",
                newName: "DeviceCredentialHash");

            migrationBuilder.RenameIndex(
                name: "IX_WateringDevices_DeviceAccessKeyHash",
                table: "WateringDevices",
                newName: "IX_WateringDevices_DeviceCredentialHash");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WateringDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "WateringDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceToken",
                table: "WateringDevices",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WateringDevices_DeviceToken",
                table: "WateringDevices",
                column: "DeviceToken",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WateringDevices_Plants_PlantId",
                table: "WateringDevices",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
