using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class FromDev : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Let this one run, it just alters an existing column to allow nulls
            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");


          /*  migrationBuilder.AddColumn<DateTime>(
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
          */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            /* migrationBuilder.DropColumn(
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
                 table: "LeadViewModels");*/

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
