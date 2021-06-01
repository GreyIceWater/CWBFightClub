using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CWBFightClub.Migrations
{
    public partial class updateAfterRemoveName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Name_NameID",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Name_NameID",
                table: "Student");

            migrationBuilder.DropTable(
                name: "Name");

            migrationBuilder.DropIndex(
                name: "IX_Student_NameID",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "NameID",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "NameID",
                table: "Account",
                newName: "StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_Account_NameID",
                table: "Account",
                newName: "IX_Account_StudentID");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Student",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Student",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Student",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Guardian",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Guardian",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Guardian",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Student_StudentID",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Guardian");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Guardian");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Guardian");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "Account",
                newName: "NameID");

            migrationBuilder.RenameIndex(
                name: "IX_Account_StudentID",
                table: "Account",
                newName: "IX_Account_NameID");

            migrationBuilder.AddColumn<int>(
                name: "NameID",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Name",
                columns: table => new
                {
                    NameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Name", x => x.NameID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_NameID",
                table: "Student",
                column: "NameID");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Name_NameID",
                table: "Account",
                column: "NameID",
                principalTable: "Name",
                principalColumn: "NameID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Name_NameID",
                table: "Student",
                column: "NameID",
                principalTable: "Name",
                principalColumn: "NameID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
