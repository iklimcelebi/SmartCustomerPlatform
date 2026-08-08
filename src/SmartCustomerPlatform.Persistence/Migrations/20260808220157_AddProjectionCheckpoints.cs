using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCustomerPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectionCheckpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectionCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Position = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionCheckpoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectionCheckpoints_ProjectionName",
                table: "ProjectionCheckpoints",
                column: "ProjectionName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectionCheckpoints");
        }
    }
}
