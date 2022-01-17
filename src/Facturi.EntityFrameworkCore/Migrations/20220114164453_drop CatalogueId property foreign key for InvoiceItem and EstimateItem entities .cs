using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class dropCatalogueIdpropertyforeignkeyforInvoiceItemandEstimateItementities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
             name: "FK_FactureItems_Catalogues_CatalogueId",
             table: "FactureItems");

           
            migrationBuilder.DropIndex(
              name: "IX_FactureItems_CatalogueId",
              table: "FactureItems");
        }
    

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
