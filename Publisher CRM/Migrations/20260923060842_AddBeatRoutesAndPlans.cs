using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddBeatRoutesAndPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeatRoutes",
                columns: table => new
                {
                    BeatRouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatRoutes", x => x.BeatRouteId);
                    table.ForeignKey(
                        name: "FK_BeatRoutes_Users_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "Users",
                        principalColumn: "RecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeatPlans",
                columns: table => new
                {
                    BeatPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BeatRouteId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Associates = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatPlans", x => x.BeatPlanId);
                    table.ForeignKey(
                        name: "FK_BeatPlans_BeatRoutes_BeatRouteId",
                        column: x => x.BeatRouteId,
                        principalTable: "BeatRoutes",
                        principalColumn: "BeatRouteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeatPlans_Users_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "Users",
                        principalColumn: "RecordId");
                });

            migrationBuilder.CreateTable(
                name: "BeatRouteCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeatRouteId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatRouteCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeatRouteCustomers_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_BeatRouteCustomers_BeatRoutes_BeatRouteId",
                        column: x => x.BeatRouteId,
                        principalTable: "BeatRoutes",
                        principalColumn: "BeatRouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanMeetings",
                columns: table => new
                {
                    PlanMeetingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeatPlanId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    MeetingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanMeetings", x => x.PlanMeetingId);
                    table.ForeignKey(
                        name: "FK_PlanMeetings_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_PlanMeetings_BeatPlans_BeatPlanId",
                        column: x => x.BeatPlanId,
                        principalTable: "BeatPlans",
                        principalColumn: "BeatPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeatPlans_BeatRouteId",
                table: "BeatPlans",
                column: "BeatRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatPlans_SalesPersonId",
                table: "BeatPlans",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatRouteCustomers_AccountId",
                table: "BeatRouteCustomers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatRouteCustomers_BeatRouteId",
                table: "BeatRouteCustomers",
                column: "BeatRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatRoutes_SalesPersonId",
                table: "BeatRoutes",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanMeetings_AccountId",
                table: "PlanMeetings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanMeetings_BeatPlanId",
                table: "PlanMeetings",
                column: "BeatPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeatRouteCustomers");

            migrationBuilder.DropTable(
                name: "PlanMeetings");

            migrationBuilder.DropTable(
                name: "BeatPlans");

            migrationBuilder.DropTable(
                name: "BeatRoutes");
        }
    }
}
