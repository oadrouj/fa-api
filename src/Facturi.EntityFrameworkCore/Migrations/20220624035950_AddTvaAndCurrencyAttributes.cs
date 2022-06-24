using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class AddTvaAndCurrencyAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AddColumn<string>(
                name: "Tva",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                defaultValue:"20%",
                nullable: true);

             migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                defaultValue:"MAD",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropColumn(
                name: "Tva",
                table: "AppInfosEntreprise");

             migrationBuilder.DropColumn(
                name: "Currency",
                table: "AppInfosEntreprise");

        }
    }
}
