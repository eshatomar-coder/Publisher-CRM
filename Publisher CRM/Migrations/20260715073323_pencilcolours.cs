using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class pencilcolours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetailerPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RPCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetailerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleMapLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreLocationQuality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedFootfall = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerProfile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentlySells = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShareOfChildrensProducts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PegasusProductFit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallPegasusFit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonForOverallFit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPegasusAvailable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceOfSupply = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuyingPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthlyPurchaseValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplaySpaceAvailable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuitableDisplayType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WillingnessToGiveVisibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisibilityRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetailerInterest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonForInterest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainObjection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchandisingSupport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradeTheRetailer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonForGrade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyObservations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Risks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opportunities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedByNSM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NSMDecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NSMRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalRetailerStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetailerPoint", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetailerPoint");
        }
    }
}
