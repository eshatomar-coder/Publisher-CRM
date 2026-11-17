using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publisher_CRM.Migrations
{
    /// <inheritdoc />
    public partial class NewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Postcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailOptOut = table.Column<bool>(type: "bit", nullable: false),
                    LayoutId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserRelatedActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemRelatedActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnsubscribedMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnsubscribedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    RecordApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRecordDuplicate = table.Column<bool>(type: "bit", nullable: false),
                    MasterPostcodesImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectedToId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectedToModule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postcodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    RecordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryLocale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmationStatus = table.Column<bool>(type: "bit", nullable: true),
                    Zuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentShiftId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextShiftId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftEffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Grouping = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Decimal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrderPreference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredUnitForDistance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalespersonCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredCurrencyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportingToId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.RecordId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postcodes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
