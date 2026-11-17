using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class AccountModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);*/
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Account");
        }
    }
}

