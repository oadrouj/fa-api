using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class removeReferencePrefixfieldandupdatethetypeofreferencefieldonDevisFactureCatalogue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropColumn(
                name: "ReferencePrefix",
                table: "Facture");

            migrationBuilder.DropColumn(
                name: "ReferencePrefix",
                table: "Devis");
           */
            migrationBuilder.DropColumn(
                name: "ReferencePrefix",
                table: "Catalogues");


            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Facture",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Devis",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Reference",
                table: "Facture",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
            /*
            migrationBuilder.AddColumn<string>(
                name: "ReferencePrefix",
                table: "Facture",
                type: "nvarchar(1)",
                nullable: true);
            */
            migrationBuilder.AlterColumn<int>(
                name: "Reference",
                table: "Devis",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            ////migrationBuilder.AddColumn<string>(
            ////    name: "ReferencePrefix",
            ////    table: "Devis",
            ////    type: "nvarchar(1)",
            ////    nullable: true);

            migrationBuilder.AddColumn<string>(
                  name: "ReferencePrefix",
                table: "Catalogues",
                type: "nvarchar(1)",
                nullable: true);
        }
    }
}
