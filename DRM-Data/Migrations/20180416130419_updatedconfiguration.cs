using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DRMData.Migrations
{
    public partial class updatedconfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logon",
                table: "Configurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Configurations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logon",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Configurations");
        }
    }
}
