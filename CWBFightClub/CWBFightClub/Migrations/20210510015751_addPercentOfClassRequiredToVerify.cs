using Microsoft.EntityFrameworkCore.Migrations;

namespace CWBFightClub.Migrations
{
    public partial class addPercentOfClassRequiredToVerify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PercentOfClassRequiredToVerify",
                table: "AppSetting",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentOfClassRequiredToVerify",
                table: "AppSetting");
        }
    }
}
