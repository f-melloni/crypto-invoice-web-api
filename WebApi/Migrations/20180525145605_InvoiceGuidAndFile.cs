using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class InvoiceGuidAndFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceGuid",
                table: "Invoices",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceGuid",
                table: "Invoices");
        }
    }
}
