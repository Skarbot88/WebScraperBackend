using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SearchTerm = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TargetUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Positions = table.Column<string>(type: "text", nullable: false),
                    SearchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalResults = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_SearchDate",
                table: "SearchResults",
                column: "SearchDate");

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_SearchTerm",
                table: "SearchResults",
                column: "SearchTerm");

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_SearchTerm_SearchDate",
                table: "SearchResults",
                columns: new[] { "SearchTerm", "SearchDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchResults");
        }
    }
}
