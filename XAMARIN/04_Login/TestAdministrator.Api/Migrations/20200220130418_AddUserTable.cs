using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAdministrator.Api.Migrations
{
    public partial class AddUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    U_Name = table.Column<string>(maxLength: 100, nullable: false),
                    U_Salt = table.Column<string>(maxLength: 24, nullable: false),
                    U_Hash = table.Column<string>(maxLength: 44, nullable: true),
                    U_LastLogin = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.U_Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
