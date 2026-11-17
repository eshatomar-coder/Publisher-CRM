using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class cinema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetailerPoint");

            migrationBuilder.CreateTable(
                name: "RetailerPointviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RPCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetailerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreLocationQuality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreLocationOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedFootfall = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentlySells = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentlySellsOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShareOfChildrensProducts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PegasusProductFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PegasusProductFitOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallPegasusFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForOverallFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPegasusAvailable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceOfSupply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyingPattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlyPurchaseValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplaySpaceAvailable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuitableDisplayType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WillingnessToGiveVisibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisibilityRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchandisingSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetailerInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainObjection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainObjectionOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeTheRetailer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyObservations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Risks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opportunities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByNSM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NSMDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NSMRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalRetailerStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetailerPointviews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetailerPointviews");

            migrationBuilder.CreateTable(
                name: "RetailerPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyingPattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentlySells = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentlySellsOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplaySpaceAvailable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedFootfall = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalRetailerStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeTheRetailer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPegasusAvailable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyObservations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainObjection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainObjectionOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchandisingSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlyPurchaseValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NSMDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NSMRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opportunities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallPegasusFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PegasusProductFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PegasusProductFitOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RPCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForOverallFit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetailerInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetailerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByNSM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Risks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShareOfChildrensProducts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceOfSupply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreLocationOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreLocationQuality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuitableDisplayType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisibilityRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WillingnessToGiveVisibility = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetailerPoint", x => x.Id);
                });
        }
    }
}
