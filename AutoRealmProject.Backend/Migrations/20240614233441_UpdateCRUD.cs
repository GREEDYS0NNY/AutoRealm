using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRealmProject.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCRUD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CarPhoto",
                table: "CarAds",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarAds",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarPhoto",
                table: "CarAds");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarAds");
        }
    }
}
