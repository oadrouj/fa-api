using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class add_devis_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<int>(type: "int", nullable: false),
                    DateEmission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EcheancePaiement = table.Column<int>(type: "int", nullable: false),
                    MessageIntroduction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PiedDePage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remise = table.Column<float>(type: "real", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devis_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DevisItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPriceHT = table.Column<int>(type: "int", nullable: false),
                    Tva = table.Column<float>(type: "real", nullable: false),
                    TotalTtc = table.Column<float>(type: "real", nullable: false),
                    DevisId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevisItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevisItems_Devis_DevisId",
                        column: x => x.DevisId,
                        principalTable: "Devis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devis_ClientId",
                table: "Devis",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DevisItems_DevisId",
                table: "DevisItems",
                column: "DevisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevisItems");

            migrationBuilder.DropTable(
                name: "Devis");
        }
    }
}
