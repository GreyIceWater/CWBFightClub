using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CWBFightClub.Migrations
{
    public partial class file_paths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaiverName",
                table: "Student");

            migrationBuilder.CreateTable(
                name: "FilePath",
                columns: table => new
                {
                    FilePathId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StudentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePath", x => x.FilePathId);
                    table.ForeignKey(
                        name: "FK_FilePath_Student_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilePath_StudentID",
                table: "FilePath",
                column: "StudentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilePath");

            migrationBuilder.AddColumn<string>(
                name: "WaiverName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
