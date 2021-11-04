using Microsoft.EntityFrameworkCore.Migrations;

namespace ManagerCovid19.Migrations
{
    public partial class Remove_Admin_Col : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Admin",
                table: "Member");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "Member",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
