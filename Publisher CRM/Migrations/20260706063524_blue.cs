using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class blue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "ConnectedToId",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "ConnectedToModule",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "Country_Code",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "MasterPostcodesImage",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "SystemModifiedTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "SystemRelatedActivityTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "UnsubscribedTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "UserModifiedTime",
                table: "Postcodes");

            migrationBuilder.DropColumn(
                name: "UserRelatedActivityTime",
                table: "Postcodes");

            migrationBuilder.RenameColumn(
                name: "UnsubscribedMode",
                table: "Postcodes",
                newName: "RecordApprovalStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordApprovalStatus",
                table: "Postcodes",
                newName: "UnsubscribedMode");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToId",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToModule",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country_Code",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MasterPostcodesImage",
                table: "Postcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemModifiedTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemRelatedActivityTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsubscribedTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserModifiedTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserRelatedActivityTime",
                table: "Postcodes",
                type: "datetime2",
                nullable: true);
        }
    }
}
