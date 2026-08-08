using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCustomerPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketSla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSlaPaused",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaPausedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaResolutionDueAt",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaResponseDueAt",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaStartedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalSlaPausedDuration",
                table: "Tickets",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSlaPaused",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaPausedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaResolutionDueAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaResponseDueAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaStartedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TotalSlaPausedDuration",
                table: "Tickets");
        }
    }
}
