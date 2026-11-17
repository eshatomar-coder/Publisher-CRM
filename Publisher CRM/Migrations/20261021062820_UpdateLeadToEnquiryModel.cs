using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeadToEnquiryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionByDate",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "CallRemark",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ClosingDate",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ContactPhoneNo",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "CostingAttachmentPath",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "CustomerRelationship",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "HistoryDate",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "HistoryRemark",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "LayoutId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "LeadSource",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "LeadSourceName",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "MarkAsNoOutput",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NewLeadName",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "NewLeadOwnerId",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderAttachmentPath",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "OrderValue",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Pipeline",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "Probability",
                table: "LeadViewModels");

            migrationBuilder.DropColumn(
                name: "PsiNo",
                table: "LeadViewModels");

            migrationBuilder.RenameColumn(
                name: "StageModifiedTime",
                table: "LeadViewModels",
                newName: "LastSalesCallDate");

            migrationBuilder.RenameColumn(
                name: "StageAttachmentPath",
                table: "LeadViewModels",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "SocialLeadId",
                table: "LeadViewModels",
                newName: "LeadOwnerId");

            migrationBuilder.RenameColumn(
                name: "SoNo",
                table: "LeadViewModels",
                newName: "LastCallRemark");

            migrationBuilder.RenameColumn(
                name: "SampleSend",
                table: "LeadViewModels",
                newName: "EnquiryType");

            migrationBuilder.RenameColumn(
                name: "SalesCallNameId",
                table: "LeadViewModels",
                newName: "EnquiryId");

            migrationBuilder.RenameColumn(
                name: "ReasonForLoss",
                table: "LeadViewModels",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "PsiValue",
                table: "LeadViewModels",
                newName: "EnquiryValue");

            migrationBuilder.RenameColumn(
                name: "PsiPicturePath",
                table: "LeadViewModels",
                newName: "CommercialTerms");

            migrationBuilder.AddColumn<int>(
                name: "SalesCallId",
                table: "LeadViewModels",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesCallId",
                table: "LeadViewModels");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "LeadViewModels",
                newName: "StageAttachmentPath");

            migrationBuilder.RenameColumn(
                name: "LeadOwnerId",
                table: "LeadViewModels",
                newName: "SocialLeadId");

            migrationBuilder.RenameColumn(
                name: "LastSalesCallDate",
                table: "LeadViewModels",
                newName: "StageModifiedTime");

            migrationBuilder.RenameColumn(
                name: "LastCallRemark",
                table: "LeadViewModels",
                newName: "SoNo");

            migrationBuilder.RenameColumn(
                name: "EnquiryValue",
                table: "LeadViewModels",
                newName: "PsiValue");

            migrationBuilder.RenameColumn(
                name: "EnquiryType",
                table: "LeadViewModels",
                newName: "SampleSend");

            migrationBuilder.RenameColumn(
                name: "EnquiryId",
                table: "LeadViewModels",
                newName: "SalesCallNameId");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "LeadViewModels",
                newName: "ReasonForLoss");

            migrationBuilder.RenameColumn(
                name: "CommercialTerms",
                table: "LeadViewModels",
                newName: "PsiPicturePath");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionByDate",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallRemark",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingDate",
                table: "LeadViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNo",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostingAttachmentPath",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerRelationship",
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
                name: "LayoutId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSource",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSourceName",
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
                name: "Mode",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewLeadName",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewLeadOwnerId",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderAttachmentPath",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderValue",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pipeline",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Probability",
                table: "LeadViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PsiNo",
                table: "LeadViewModels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
