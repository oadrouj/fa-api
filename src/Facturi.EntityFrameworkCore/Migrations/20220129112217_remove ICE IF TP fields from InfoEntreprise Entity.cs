using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class removeICEIFTPfieldsfromInfoEntrepriseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ICE",
                table: "AppInfosEntreprise");

            migrationBuilder.DropColumn(
                name: "IF",
                table: "AppInfosEntreprise");

            migrationBuilder.DropColumn(
                name: "TP",
                table: "AppInfosEntreprise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ICE",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IF",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TP",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
