using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class Saviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNameId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assistant",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AsstPhone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AverageTimeSpentMinutes",
                table: "Contacts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampaignSourceId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeLogTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToModule",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactImage",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactOwnerId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNo",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DaysVisited",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailOptOut",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EnrichStatus",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Contacts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Contacts",
                type: "nvarchar(max)",
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

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomePhone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecordDuplicate",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEnrichedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LayoutId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSource",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MailingCity",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingCountry",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingState",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingStreet",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingZip",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
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
                name: "NegativeScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NegativeTouchPointScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfChats",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherCity",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherCountry",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPhone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherState",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherStreet",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherZip",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositiveScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositiveTouchPointScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordApprovalStatus",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordCreationSourceId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordStatus",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportingToId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkypeID",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialLeadId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemModifiedTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemRelatedActivityTime",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouchPointScore",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnsubscribedMode",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsubscribedTime",
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

            migrationBuilder.AddColumn<string>(
                name: "VendorNameId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisitorScore",
                table: "Contacts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNameId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Assistant",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "AsstPhone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "AverageTimeSpentMinutes",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CampaignSourceId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ChangeLogTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ConnectedToId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ConnectedToModule",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactImage",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactOwnerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CustomerNo",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "DaysVisited",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "EmailOptOut",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "EnrichStatus",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FirstPageVisited",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FirstVisit",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "HomePhone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "IsRecordDuplicate",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LastEnrichedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LayoutId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LeadScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LeadSource",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MailingCity",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MailingCountry",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MailingState",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MailingStreet",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MailingZip",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MostRecentVisit",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NegativeScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NegativeTouchPointScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NumberOfChats",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherCity",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherCountry",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherPhone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherState",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherStreet",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OtherZip",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "PositiveScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "PositiveTouchPointScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RecordApprovalStatus",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RecordCreationSourceId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ReportingToId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SkypeID",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SocialLeadId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SystemModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SystemRelatedActivityTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "TouchPointScore",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UnsubscribedMode",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UnsubscribedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserModifiedTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserRelatedActivityTime",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "VendorNameId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "VisitorScore",
                table: "Contacts");
        }
    }
}
