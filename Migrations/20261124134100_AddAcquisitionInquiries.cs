using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMobiNardo.Migrations
{
    /// <inheritdoc />
    public partial class AddAcquisitionInquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcquisitionInquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    Company = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcquisitionInquiries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcquisitionInquiries_CreatedAtUtc",
                table: "AcquisitionInquiries",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcquisitionInquiries");
        }
    }
}
