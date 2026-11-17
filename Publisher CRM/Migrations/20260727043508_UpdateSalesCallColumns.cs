using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalesCallColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CallType",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HistoryDate",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoryRemark",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSourceName",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleSend",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallType",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "HistoryDate",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "HistoryRemark",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "LeadSourceName",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SampleSend",
                table: "LeadViewModels");

            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
