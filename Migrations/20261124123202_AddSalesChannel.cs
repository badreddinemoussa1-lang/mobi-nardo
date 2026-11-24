using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMobiNardo.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Sales",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "À préciser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Sales");
        }
    }
}
