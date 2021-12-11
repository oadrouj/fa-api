using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class addcurrencyfieldtoDevisandFactureentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Facture",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Devis",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Devis");
        }
    }
}
