using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Facturi.Migrations
{
    public partial class AddabpauditingpropertiestoFactureInfosPaiemententity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "FactureInfosPaiements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "FactureInfosPaiements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "FactureInfosPaiements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "FactureInfosPaiements",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "FactureInfosPaiements");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "FactureInfosPaiements");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "FactureInfosPaiements");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "FactureInfosPaiements");
        }
    }
}
