using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class barbie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeadViewModels",
                columns: table => new
                {
                    RecordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NewLeadName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewLeadOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountNameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Probability = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpectedRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NextStep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeadSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampaignSourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pipeline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForecastCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocialLeadId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewLeadImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerRelationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FollowupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionToBeTaken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: true),
                    PositiveScore = table.Column<int>(type: "int", nullable: true),
                    NegativeScore = table.Column<int>(type: "int", nullable: true),
                    TouchPointScore = table.Column<int>(type: "int", nullable: true),
                    PositiveTouchPointScore = table.Column<int>(type: "int", nullable: true),
                    NegativeTouchPointScore = table.Column<int>(type: "int", nullable: true),
                    OrderToBeSuppliedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PsiValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PsiNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesCallNameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectedToId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectedToModule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForLoss = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordCreationSourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayoutId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeadConversionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StageModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemRelatedActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserRelatedActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangeLogTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SalesCycleDuration = table.Column<int>(type: "int", nullable: true),
                    OverallSalesDuration = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadViewModels", x => x.RecordId);
                });

            migrationBuilder.CreateTable(
                name: "orderManagementViewModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoAttachmentSavedPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoAttachmentCloudUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesCallName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderManagementOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JpgAttachmentSavedPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JpgAttachmentCloudUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderManagementViewModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadViewModels");

            migrationBuilder.DropTable(
                name: "orderManagementViewModels");
        }
    }
}
