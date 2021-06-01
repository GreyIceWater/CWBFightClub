using Microsoft.EntityFrameworkCore.Migrations;

namespace CWBFightClub.Migrations
{
    public partial class updateForCalendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recurrence",
                table: "ScheduledClass");

            migrationBuilder.AddColumn<bool>(
                name: "HasRecurrence",
                table: "ScheduledClass",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceFrequency",
                table: "ScheduledClass",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceTime",
                table: "ScheduledClass",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CalendarColor",
                table: "Discipline",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRecurrence",
                table: "ScheduledClass");

            migrationBuilder.DropColumn(
                name: "RecurrenceFrequency",
                table: "ScheduledClass");

            migrationBuilder.DropColumn(
                name: "RecurrenceTime",
                table: "ScheduledClass");

            migrationBuilder.DropColumn(
                name: "CalendarColor",
                table: "Discipline");

            migrationBuilder.AddColumn<string>(
                name: "Recurrence",
                table: "ScheduledClass",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
