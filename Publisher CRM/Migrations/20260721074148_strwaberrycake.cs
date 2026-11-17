using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class strwaberrycake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventMasterOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfCustomerMet = table.Column<int>(type: "int", nullable: false),
                    SalespersonCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventStatus = table.Column<int>(type: "int", nullable: false),
                    RentalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TravelCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DesigningAndSetupCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventCloseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMaster", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventMaster");
        }
    }
}
