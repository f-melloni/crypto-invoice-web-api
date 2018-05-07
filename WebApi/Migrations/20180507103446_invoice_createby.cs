using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class invoice_createby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "createdById",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_createdById",
                table: "Invoices",
                column: "createdById");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_createdById",
                table: "Invoices",
                column: "createdById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_createdById",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_createdById",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "createdById",
                table: "Invoices");
        }
    }
}
