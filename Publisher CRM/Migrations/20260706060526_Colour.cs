using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class Colour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Postcodes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecordStatus",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "Postcodes");
        }
    }
}
