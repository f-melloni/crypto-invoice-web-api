using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Migrations
{
    public partial class initial_models_entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BTCAddress = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ETHVS = table.Column<string>(nullable: true),
                    FiatAmount = table.Column<string>(nullable: true),
                    FiatCurrencyCode = table.Column<string>(nullable: true),
                    FixedRateOnCreation = table.Column<bool>(nullable: false),
                    LTCAddress = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NewFixER_BTC = table.Column<double>(nullable: true),
                    NewFixER_ETH = table.Column<double>(nullable: true),
                    NewFixER_LTC = table.Column<double>(nullable: true),
                    NewFixER_XMR = table.Column<double>(nullable: true),
                    OldFixER_BTC = table.Column<double>(nullable: true),
                    OldFixER_ETH = table.Column<double>(nullable: true),
                    OldFixER_LTC = table.Column<double>(nullable: true),
                    OldFixER_XMR = table.Column<double>(nullable: true),
                    XMRVS = table.Column<string>(nullable: true),
                    state = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Level = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "LogItems");
        }
    }
}
