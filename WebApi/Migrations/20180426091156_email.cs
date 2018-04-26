using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Emails",
                newName: "SmtpError");

            migrationBuilder.AddColumn<string>(
                name: "AttachmenList",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInserted",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Emails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmenList",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "DateInserted",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Emails");

            migrationBuilder.RenameColumn(
                name: "SmtpError",
                table: "Emails",
                newName: "Name");
        }
    }
}
