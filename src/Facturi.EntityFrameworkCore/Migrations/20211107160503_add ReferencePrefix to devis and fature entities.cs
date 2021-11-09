using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class addReferencePrefixtodevisandfatureentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferencePrefix",
                table: "Facture",
                type: "nvarchar(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferencePrefix",
                table: "Devis",
                type: "nvarchar(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferencePrefix",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "ReferencePrefix",
                table: "Devis");
        }
    }
}
