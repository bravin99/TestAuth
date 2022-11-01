using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAuth.Server.Data.Migrations
{
    public partial class Usermodeltoenfieldsupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "Tokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpires",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Expires",
                table: "Tokens",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenExpires",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Expires",
                table: "Tokens");

            migrationBuilder.AddColumn<double>(
                name: "DurationInMinutes",
                table: "Tokens",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
