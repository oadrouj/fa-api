using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class addStatutJuridiquefieldtoInfosEntrepriseentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatutJuridique",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatutJuridique",
                table: "AppInfosEntreprise");
        }
    }
}
