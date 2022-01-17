using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class makeCatalogueIdpropertyaforeignkeyforInvoiceItemandEstimateItementitiesfixforeignkeyconflicts5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_FactureItems_CatalogueId",
            //    table: "FactureItems");

           
            migrationBuilder.DropForeignKey(
              name: "FK_FactureItems_Catalogues_CatalogueId",
              table: "FactureItems");

            //migrationBuilder.DropColumn(
            //   name: "CatalogueId",
            //   table: "FactureItems");

            migrationBuilder.DropIndex(
              name: "IX_FactureItems_CatalogueId",
              table: "FactureItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FactureItems_CatalogueId",
                table: "FactureItems");

            migrationBuilder.CreateIndex(
                name: "IX_FactureItems_CatalogueId",
                table: "FactureItems",
                column: "CatalogueId");
        }
    }
}
