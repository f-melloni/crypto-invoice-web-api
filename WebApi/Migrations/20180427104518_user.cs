using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BTCXPUB",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ETHAccount",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LTCXPUB",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XMRAddress",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XMRPrivateViewKey",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XMRPublicViewKey",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BTCXPUB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ETHAccount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LTCXPUB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "XMRAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "XMRPrivateViewKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "XMRPublicViewKey",
                table: "AspNetUsers");
        }
    }
}
