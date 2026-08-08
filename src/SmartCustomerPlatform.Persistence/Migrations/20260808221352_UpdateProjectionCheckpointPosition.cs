using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCustomerPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectionCheckpointPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "ProjectionCheckpoints",
                newName: "PreparePosition");

            migrationBuilder.AddColumn<decimal>(
                name: "CommitPosition",
                table: "ProjectionCheckpoints",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitPosition",
                table: "ProjectionCheckpoints");

            migrationBuilder.RenameColumn(
                name: "PreparePosition",
                table: "ProjectionCheckpoints",
                newName: "Position");
        }
    }
}
