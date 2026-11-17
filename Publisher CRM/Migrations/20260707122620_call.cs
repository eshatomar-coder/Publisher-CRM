using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class call : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Salescall",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesCallType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contacts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pipeline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRelationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerGeography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerProductProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PotentialOrder = table.Column<bool>(type: "bit", nullable: false),
                    SampleRequired = table.Column<bool>(type: "bit", nullable: false),
                    HasOrder = table.Column<bool>(type: "bit", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salescall", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salescall");
        }
    }
}
