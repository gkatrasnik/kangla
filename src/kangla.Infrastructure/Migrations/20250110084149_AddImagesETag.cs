using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kangla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesETag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Images");
        }
    }
}
