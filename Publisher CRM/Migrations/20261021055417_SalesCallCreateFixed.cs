using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class SalesCallCreateFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "Salescall",
                newName: "CallSubType");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "Salescall",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Associates",
                table: "Salescall",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Salescall",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Associates",
                table: "Salescall");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Salescall");

            migrationBuilder.RenameColumn(
                name: "CallSubType",
                table: "Salescall",
                newName: "Mode");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "Salescall",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
