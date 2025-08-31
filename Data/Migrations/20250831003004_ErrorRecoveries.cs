using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ErrorRecoveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorRecoveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalizationKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ErrorContent = table.Column<string>(type: "text", nullable: false),
                    ErrorResponse = table.Column<string>(type: "text", nullable: false),
                    ErrorResponseStatusCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorRecoveries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorRecoveries_NormalizationKey",
                table: "ErrorRecoveries",
                column: "NormalizationKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorRecoveries");
        }
    }
}
