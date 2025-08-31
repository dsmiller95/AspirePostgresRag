using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ErrorJsonSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorContentSummary",
                table: "ErrorRecoveries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ErrorRecoverySchemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalizationKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    JsonSchema = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorRecoverySchemas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorRecoverySchemas_NormalizationKey",
                table: "ErrorRecoverySchemas",
                column: "NormalizationKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorRecoverySchemas");

            migrationBuilder.DropColumn(
                name: "ErrorContentSummary",
                table: "ErrorRecoveries");
        }
    }
}
