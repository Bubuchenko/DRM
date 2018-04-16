using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DRMData.Migrations
{
    public partial class _160420181 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Configurations_ConfigurationID",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ConfigurationID",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ConfigurationID",
                table: "Applications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfigurationID",
                table: "Applications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ConfigurationID",
                table: "Applications",
                column: "ConfigurationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Configurations_ConfigurationID",
                table: "Applications",
                column: "ConfigurationID",
                principalTable: "Configurations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
