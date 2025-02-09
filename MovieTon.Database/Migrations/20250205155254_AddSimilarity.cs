using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTon.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSimilarity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbSimilarity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SimilarToId = table.Column<int>(type: "int", nullable: false),
                    SimilarFromId = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSimilarity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbSimilarity_Movies_SimilarFromId",
                        column: x => x.SimilarFromId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbSimilarity_Movies_SimilarToId",
                        column: x => x.SimilarToId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DbSimilarity_SimilarFromId",
                table: "DbSimilarity",
                column: "SimilarFromId");

            migrationBuilder.CreateIndex(
                name: "IX_DbSimilarity_SimilarToId",
                table: "DbSimilarity",
                column: "SimilarToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbSimilarity");
        }
    }
}
