using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class NewFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnquiryCount",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IssueCount",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCount",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemarkCount",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TwelveMonthBusiness",
                table: "Account",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "EnquiryCount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IssueCount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "OrderCount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RemarkCount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "TwelveMonthBusiness",
                table: "Account");
        }
    }
}
