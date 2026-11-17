using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class DailyPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPlans",
                columns: table => new
                {
                    DailyPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    CallType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Associates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlans", x => x.DailyPlanId);
                    table.ForeignKey(
                        name: "FK_DailyPlans_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "RecordId");
                    table.ForeignKey(
                        name: "FK_DailyPlans_Users_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "Users",
                        principalColumn: "RecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyPlanContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyPlanId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlanContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPlanContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_DailyPlanContacts_DailyPlans_DailyPlanId",
                        column: x => x.DailyPlanId,
                        principalTable: "DailyPlans",
                        principalColumn: "DailyPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyPlanCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyPlanId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlanCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPlanCustomers_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_DailyPlanCustomers_DailyPlans_DailyPlanId",
                        column: x => x.DailyPlanId,
                        principalTable: "DailyPlans",
                        principalColumn: "DailyPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanContacts_ContactId",
                table: "DailyPlanContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanContacts_DailyPlanId",
                table: "DailyPlanContacts",
                column: "DailyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanCustomers_AccountId",
                table: "DailyPlanCustomers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanCustomers_DailyPlanId",
                table: "DailyPlanCustomers",
                column: "DailyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_CreatedById",
                table: "DailyPlans",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_SalesPersonId",
                table: "DailyPlans",
                column: "SalesPersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlanContacts");

            migrationBuilder.DropTable(
                name: "DailyPlanCustomers");

            migrationBuilder.DropTable(
                name: "DailyPlans");
        }
    }
}
