using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiWithAuth.Migrations
{
    public partial class sessiontokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccessToken = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTokens", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionTokens");
        }
    }
}
