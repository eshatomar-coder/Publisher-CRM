using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class doremon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CostingAttachmentPath",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MarkAsNoOutput",
                table: "LeadViewModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderAttachmentPath",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostingAttachmentPath",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "MarkAsNoOutput",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderAttachmentPath",
                table: "LeadViewModels");
        }
    }
}
