using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    ProfileNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balances = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.ProfileNumber);
                });

            migrationBuilder.CreateTable(
                name: "SessionKeys",
                columns: table => new
                {
                    SessionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientWriteKeyBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServerWriteKeyBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientWriteMacKeyBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServerWriteMacKeyBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionKeys", x => x.SessionID);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Payer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "BankingInfos",
                columns: table => new
                {
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankWebsite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileNumber = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingInfos", x => x.BankId);
                    table.ForeignKey(
                        name: "FK_BankingInfos_BankAccounts_ProfileNumber",
                        column: x => x.ProfileNumber,
                        principalTable: "BankAccounts",
                        principalColumn: "ProfileNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "ProfileNumber", "Balances", "Name" },
                values: new object[] { "1255070770448", 999, "Client" });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "ProfileNumber", "Balances", "Name" },
                values: new object[] { "7467811997849", 999, "Merchant" });

            migrationBuilder.InsertData(
                table: "BankingInfos",
                columns: new[] { "BankId", "BankLocation", "BankWebsite", "Name", "ProfileNumber" },
                values: new object[] { new Guid("40822906-84f0-4dae-b8f0-696fce457db8"), "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Merchant", "www.merchant.com", "Merchant Duy", "7467811997849" });

            migrationBuilder.InsertData(
                table: "BankingInfos",
                columns: new[] { "BankId", "BankLocation", "BankWebsite", "Name", "ProfileNumber" },
                values: new object[] { new Guid("dbe552e4-37de-4ffb-b920-ab7caa9ebe0d"), "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Client", "www.client.com", "Client Duy", "1255070770448" });

            migrationBuilder.CreateIndex(
                name: "IX_BankingInfos_ProfileNumber",
                table: "BankingInfos",
                column: "ProfileNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankingInfos");

            migrationBuilder.DropTable(
                name: "SessionKeys");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
