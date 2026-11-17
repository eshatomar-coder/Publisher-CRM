using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class OrderManagmentRevised : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_orderManagementViewModels",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryDate",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "PoValue",
                table: "orderManagementViewModels");

            migrationBuilder.RenameColumn(
                name: "SalesCallName",
                table: "orderManagementViewModels",
                newName: "AccountNameId");

            migrationBuilder.RenameColumn(
                name: "PoAttachmentSavedPath",
                table: "orderManagementViewModels",
                newName: "TitleStatus");

            migrationBuilder.RenameColumn(
                name: "PoAttachmentCloudUrl",
                table: "orderManagementViewModels",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "JpgAttachmentSavedPath",
                table: "orderManagementViewModels",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "JpgAttachmentCloudUrl",
                table: "orderManagementViewModels",
                newName: "CustomerProjectCoordinatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OrderManagementOwnerId",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "orderManagementViewModels",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AgreementPoFilePath",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BcSoDate",
                table: "orderManagementViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BcSoNo",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialTerms",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNameId",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "orderManagementViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "orderManagementViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderValue",
                table: "orderManagementViewModels",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProjectParked",
                table: "orderManagementViewModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SalesCallId",
                table: "orderManagementViewModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TentativeDispatchDate",
                table: "orderManagementViewModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderManagementViewModels",
                table: "orderManagementViewModels",
                column: "RecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_orderManagementViewModels",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "AgreementPoFilePath",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "BcSoDate",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "BcSoNo",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "CommercialTerms",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "ContactNameId",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "OrderValue",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "ProjectParked",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "SalesCallId",
                table: "orderManagementViewModels");

            migrationBuilder.DropColumn(
                name: "TentativeDispatchDate",
                table: "orderManagementViewModels");

            migrationBuilder.RenameColumn(
                name: "TitleStatus",
                table: "orderManagementViewModels",
                newName: "PoAttachmentSavedPath");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "orderManagementViewModels",
                newName: "PoAttachmentCloudUrl");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "orderManagementViewModels",
                newName: "JpgAttachmentSavedPath");

            migrationBuilder.RenameColumn(
                name: "CustomerProjectCoordinatorId",
                table: "orderManagementViewModels",
                newName: "JpgAttachmentCloudUrl");

            migrationBuilder.RenameColumn(
                name: "AccountNameId",
                table: "orderManagementViewModels",
                newName: "SalesCallName");

            migrationBuilder.AlterColumn<int>(
                name: "Stage",
                table: "orderManagementViewModels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderManagementOwnerId",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "orderManagementViewModels",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "orderManagementViewModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "orderManagementViewModels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "PoValue",
                table: "orderManagementViewModels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderManagementViewModels",
                table: "orderManagementViewModels",
                column: "Id");
        }
    }
}
