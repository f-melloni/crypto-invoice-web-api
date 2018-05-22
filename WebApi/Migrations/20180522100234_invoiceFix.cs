using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class invoiceFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldFixER_BTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OldFixER_ETH",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OldFixER_LTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OldFixER_XMR",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "FixedRateOnCreation",
                table: "Invoices",
                newName: "AcceptXMR");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptBTC",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptETH",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AcceptLTC",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "AcceptXMR",
                table: "Invoices",
                newName: "FixedRateOnCreation");

            migrationBuilder.AddColumn<double>(
                name: "OldFixER_BTC",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OldFixER_ETH",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OldFixER_LTC",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OldFixER_XMR",
                table: "Invoices",
                nullable: true);
        }
    }
}
