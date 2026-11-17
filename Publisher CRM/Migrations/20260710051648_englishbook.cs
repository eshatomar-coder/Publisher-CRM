using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class englishbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageTimeSpentMinutes",
                table: "Contacts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactOwnerId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DaysVisited",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstPageVisited",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstVisit",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LayoutId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MostRecentVisit",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfChats",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPhone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salutation",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemModifiedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserModifiedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserRelatedActivityTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VisitorScore",
                table: "Contacts",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageTimeSpentMinutes",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactOwnerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "DaysVisited",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FirstPageVisited",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FirstVisit",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LayoutId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MostRecentVisit",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NumberOfChats",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherPhone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Salutation",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SystemModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserRelatedActivityTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "VisitorScore",
                table: "Contacts");
        }
    }
}
