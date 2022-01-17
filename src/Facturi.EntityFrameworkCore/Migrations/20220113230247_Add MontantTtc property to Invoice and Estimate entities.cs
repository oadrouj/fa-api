using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class AddMontantTtcpropertytoInvoiceandEstimateentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MontantTtc",
                table: "Facture",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MontantTtc",
                table: "Devis",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontantTtc",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "MontantTtc",
                table: "Devis");
        }
    }
}
