using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class addIntroMessageandFooterfieldtoInfosEntrepriseentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "HasLogo",
                table: "AppInfosEntreprise",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Footer",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntroMessage",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Footer",
                table: "AppInfosEntreprise");

            migrationBuilder.DropColumn(
                name: "IntroMessage",
                table: "AppInfosEntreprise");

            migrationBuilder.AlterColumn<bool>(
                name: "HasLogo",
                table: "AppInfosEntreprise",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
