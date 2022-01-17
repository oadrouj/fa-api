using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class makeCatalogueIdpropertyaforeignkeyforInvoiceItemandEstimateItementitiesfixforeignkeyconflicts4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<long>(
                name: "CatalogueId",
                table: "FactureItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_FactureItems_Catalogues_CatalogueId",
                table: "FactureItems",
                column: "CatalogueId",
                principalTable: "Catalogues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
              name: "IX_FactureItems_CatalogueId",
              table: "FactureItems",
              column: "CatalogueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FactureItems_Catalogues_CatalogueId",
                table: "FactureItems");

            migrationBuilder.DropColumn(
               name: "CatalogueId",
               table: "FactureItems");

        }
    }
}
