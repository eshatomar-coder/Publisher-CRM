using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class applenew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionToBeTaken",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "CampaignSourceId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ChangeLogTime",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ConnectedToId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ConnectedToModule",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ExpectedRevenue",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ForecastCategory",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NegativeScore",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NegativeTouchPointScore",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NewLeadImagePath",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NextStep",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderToBeSuppliedById",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "PositiveScore",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "PositiveTouchPointScore",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "RecordCreationSourceId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SocialLeadId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SystemModifiedTime",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "SystemRelatedActivityTime",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "TouchPointScore",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "UserRelatedActivityTime",
                table: "LeadViewModels");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "LeadViewModels",
                newName: "StageAttachmentPath");

            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "LeadViewModels",
                newName: "PsiPicturePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StageAttachmentPath",
                table: "LeadViewModels",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "PsiPicturePath",
                table: "LeadViewModels",
                newName: "Tag");

            migrationBuilder.AddColumn<string>(
                name: "ActionToBeTaken",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampaignSourceId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeLogTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectedToModule",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedRevenue",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForecastCategory",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "LeadViewModels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NegativeScore",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NegativeTouchPointScore",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewLeadImagePath",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextStep",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderToBeSuppliedById",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositiveScore",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositiveTouchPointScore",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordCreationSourceId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordStatus",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialLeadId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemModifiedTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SystemRelatedActivityTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouchPointScore",
                table: "LeadViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserRelatedActivityTime",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);
        }
    }
}
