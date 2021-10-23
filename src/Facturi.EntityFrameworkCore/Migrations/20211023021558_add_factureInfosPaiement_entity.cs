using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class add_factureInfosPaiement_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FactureInfosPaiements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePaiement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantPaye = table.Column<float>(type: "real", nullable: false),
                    ModePaiement = table.Column<int>(type: "int", nullable: false),
                    FactureId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureInfosPaiements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactureInfosPaiements_Facture_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Facture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactureInfosPaiements_FactureId",
                table: "FactureInfosPaiements",
                column: "FactureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactureInfosPaiements");
        }
    }
}
