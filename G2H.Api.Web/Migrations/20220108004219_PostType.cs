using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2H.Api.Web.Migrations
{
    public partial class PostType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "PostTypes",
                newName: "IsEnabled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "PostTypes",
                newName: "IsActive");
        }
    }
}
