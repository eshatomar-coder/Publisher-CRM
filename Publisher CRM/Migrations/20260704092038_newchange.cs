using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class newchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Account",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyRegNoCIN",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerActivity",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerChannel",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerGeography",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerProductProfile",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerZone",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ERPCustomerCode",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ERPStatus",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrequencyOfOrder",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GSTRegistrationNo",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "InitialOrderValue",
                table: "Account",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NoBranchesOutlets",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanNo",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerm",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalespersonCode",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YearlyBusinessExpected",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearsInOperation",
                table: "Account",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CompanyRegNoCIN",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerActivity",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerChannel",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerGeography",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerProductProfile",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerZone",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ERPCustomerCode",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ERPStatus",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "FrequencyOfOrder",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "GSTRegistrationNo",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "InitialOrderValue",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "NoBranchesOutlets",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PanNo",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PaymentTerm",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SalespersonCode",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "YearlyBusinessExpected",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "YearsInOperation",
                table: "Account");
        }
    }
}
