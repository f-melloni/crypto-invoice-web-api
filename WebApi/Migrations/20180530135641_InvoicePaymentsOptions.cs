using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class InvoicePaymentsOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_createdById",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptBTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptETH",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptLTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptXMR",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BTCAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ETHVS",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "LTCAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NewFixER_BTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NewFixER_ETH",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NewFixER_LTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NewFixER_XMR",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "XMRVS",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "Invoices",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "createdById",
                table: "Invoices",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_createdById",
                table: "Invoices",
                newName: "IX_Invoices_CreatedById");

            migrationBuilder.CreateTable(
                name: "InvoicePayment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(maxLength: 10, nullable: false),
                    ExchangeRate = table.Column<double>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: false),
                    VarSymbol = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayment_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayment_InvoiceId",
                table: "InvoicePayment",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_CreatedById",
                table: "Invoices",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_CreatedById",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "InvoicePayment");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Invoices",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Invoices",
                newName: "createdById");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CreatedById",
                table: "Invoices",
                newName: "IX_Invoices_createdById");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptBTC",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptETH",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptLTC",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptXMR",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BTCAddress",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ETHVS",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LTCAddress",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NewFixER_BTC",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NewFixER_ETH",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NewFixER_LTC",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NewFixER_XMR",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XMRVS",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_createdById",
                table: "Invoices",
                column: "createdById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
