using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankingInfos",
                columns: table => new
                {
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingInfos", x => x.BankId);
                });

            migrationBuilder.InsertData(
                table: "BankingInfos",
                columns: new[] { "BankId", "BankLocation", "ProfileNumber" },
                values: new object[] { new Guid("40822906-84f0-4dae-b8f0-696fce457db8"), "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Merchant", "7467811997849" });

            migrationBuilder.InsertData(
                table: "BankingInfos",
                columns: new[] { "BankId", "BankLocation", "ProfileNumber" },
                values: new object[] { new Guid("dbe552e4-37de-4ffb-b920-ab7caa9ebe0d"), "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Client", "1255070770448" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankingInfos");
        }
    }
}
