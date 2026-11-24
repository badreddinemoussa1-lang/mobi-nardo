using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMobiNardo.Migrations
{
    /// <inheritdoc />
    public partial class AddMotorcycleCatalogFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Motorcycles",
                type: "TEXT",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Motorcycles",
                type: "TEXT",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoFileName",
                table: "Motorcycles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Motorcycles");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Motorcycles");

            migrationBuilder.DropColumn(
                name: "PhotoFileName",
                table: "Motorcycles");
        }
    }
}
