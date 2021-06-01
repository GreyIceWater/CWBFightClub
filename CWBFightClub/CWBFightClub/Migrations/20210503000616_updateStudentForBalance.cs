using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CWBFightClub.Migrations
{
    public partial class updateStudentForBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AltPaymentAgreementNote",
                table: "Student",
                newName: "PaymentAgreementNote");

            migrationBuilder.RenameColumn(
                name: "AltPaymentAgreementAmount",
                table: "Student",
                newName: "PaymentAgreementAmount");

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceModifiedBySystemDate",
                table: "Student",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentAgreenmentPeriod",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceModifiedBySystemDate",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "PaymentAgreenmentPeriod",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "PaymentAgreementNote",
                table: "Student",
                newName: "AltPaymentAgreementNote");

            migrationBuilder.RenameColumn(
                name: "PaymentAgreementAmount",
                table: "Student",
                newName: "AltPaymentAgreementAmount");
        }
    }
}
