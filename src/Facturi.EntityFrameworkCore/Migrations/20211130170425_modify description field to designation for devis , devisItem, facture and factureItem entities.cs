using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class modifydescriptionfieldtodesignationfordevisdevisItemfactureandfactureItementities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "FactureItems",
                newName: "Designation");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "DevisItems",
                newName: "Designation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Designation",
                table: "FactureItems",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Designation",
                table: "DevisItems",
                newName: "Description");
        }
    }
}
