using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class AddExchangeRateModetoInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FiatCurrencyCode",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateMode",
                table: "Invoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRateMode",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FiatCurrencyCode",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
