using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMobiNardo.Migrations
{
    /// <inheritdoc />
    public partial class AddWhiteLabelBusinessProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 240, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 24, nullable: true),
                    WhatsApp = table.Column<string>(type: "TEXT", maxLength: 24, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryColor = table.Column<string>(type: "TEXT", nullable: false),
                    GoogleMapsUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OpeningHours = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    InstagramUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TikTokUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FacebookUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OfficialWebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LiveCatalogUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    FiscalIdentifier = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ProfessionalTaxNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Ice = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    CommercialRegister = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    TaxRegime = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessProfiles_Id",
                table: "BusinessProfiles",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessProfiles");
        }
    }
}
