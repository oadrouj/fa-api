using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class addIntroMessageandFooterforEstimateandInvoiceinInfosEntrepriceentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntroMessage",
                table: "AppInfosEntreprise",
                newName: "InvoiceIntroMessage");

            migrationBuilder.RenameColumn(
                name: "Footer",
                table: "AppInfosEntreprise",
                newName: "InvoiceFooter");

            migrationBuilder.AddColumn<string>(
                name: "EstimateFooter",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstimateIntroMessage",
                table: "AppInfosEntreprise",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimateFooter",
                table: "AppInfosEntreprise");

            migrationBuilder.DropColumn(
                name: "EstimateIntroMessage",
                table: "AppInfosEntreprise");

            migrationBuilder.RenameColumn(
                name: "InvoiceIntroMessage",
                table: "AppInfosEntreprise",
                newName: "IntroMessage");

            migrationBuilder.RenameColumn(
                name: "InvoiceFooter",
                table: "AppInfosEntreprise",
                newName: "Footer");
        }
    }
}
