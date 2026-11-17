using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class food : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "LeadConversionTime",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OverallSalesDuration",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SalesCycleDuration",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "UserModifiedTime",
                table: "LeadViewModels");

            migrationBuilder.AddColumn<decimal>(
                name: "OrderValue",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialLeadId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderValue",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SocialLeadId",
                table: "LeadViewModels");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LeadConversionTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OverallSalesDuration",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesCycleDuration",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserModifiedTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);
        }
    }
}
