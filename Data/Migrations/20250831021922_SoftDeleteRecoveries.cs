using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteRecoveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ErrorRecoveries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.Sql(
"""
UPDATE "PostgresRagDb".public."ErrorRecoveries" 
SET "Active" = true
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "ErrorRecoveries");
        }
    }
}
