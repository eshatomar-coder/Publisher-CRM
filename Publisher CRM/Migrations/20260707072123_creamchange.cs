using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class creamchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostCodeId",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Account",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_PostCodeId",
                table: "Account",
                column: "PostCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserId",
                table: "Account",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Postcodes_PostCodeId",
                table: "Account",
                column: "PostCodeId",
                principalTable: "Postcodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Users_UserId",
                table: "Account",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "RecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Postcodes_PostCodeId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Users_UserId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_PostCodeId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_UserId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PostCodeId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Account");
        }
    }
}
