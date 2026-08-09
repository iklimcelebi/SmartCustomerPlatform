using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCustomerPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectionCheckpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectionCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastProcessedOccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastProcessedMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionCheckpoints", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectionCheckpoints");
        }
    }
}
