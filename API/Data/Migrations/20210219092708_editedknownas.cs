using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class editedknownas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnowAs",
                table: "AspNetUsers",
                newName: "KnownAs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnownAs",
                table: "AspNetUsers",
                newName: "KnowAs");
        }
    }
}
