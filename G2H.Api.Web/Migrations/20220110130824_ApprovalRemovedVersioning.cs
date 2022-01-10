using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2H.Api.Web.Migrations
{
    public partial class ApprovalRemovedVersioning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessKey",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "IsAuditRecord",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Approvals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BusinessKey",
                table: "Approvals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsAuditRecord",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Approvals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
