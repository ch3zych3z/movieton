using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTon.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSimilarity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbSimilarity_Movies_SimilarFromId",
                table: "DbSimilarity");

            migrationBuilder.DropForeignKey(
                name: "FK_DbSimilarity_Movies_SimilarToId",
                table: "DbSimilarity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DbSimilarity",
                table: "DbSimilarity");

            migrationBuilder.RenameTable(
                name: "DbSimilarity",
                newName: "Similarities");

            migrationBuilder.RenameIndex(
                name: "IX_DbSimilarity_SimilarToId",
                table: "Similarities",
                newName: "IX_Similarities_SimilarToId");

            migrationBuilder.RenameIndex(
                name: "IX_DbSimilarity_SimilarFromId",
                table: "Similarities",
                newName: "IX_Similarities_SimilarFromId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Similarities",
                table: "Similarities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Similarities_Movies_SimilarFromId",
                table: "Similarities",
                column: "SimilarFromId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Similarities_Movies_SimilarToId",
                table: "Similarities",
                column: "SimilarToId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Similarities_Movies_SimilarFromId",
                table: "Similarities");

            migrationBuilder.DropForeignKey(
                name: "FK_Similarities_Movies_SimilarToId",
                table: "Similarities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Similarities",
                table: "Similarities");

            migrationBuilder.RenameTable(
                name: "Similarities",
                newName: "DbSimilarity");

            migrationBuilder.RenameIndex(
                name: "IX_Similarities_SimilarToId",
                table: "DbSimilarity",
                newName: "IX_DbSimilarity_SimilarToId");

            migrationBuilder.RenameIndex(
                name: "IX_Similarities_SimilarFromId",
                table: "DbSimilarity",
                newName: "IX_DbSimilarity_SimilarFromId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSimilarity",
                table: "DbSimilarity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DbSimilarity_Movies_SimilarFromId",
                table: "DbSimilarity",
                column: "SimilarFromId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DbSimilarity_Movies_SimilarToId",
                table: "DbSimilarity",
                column: "SimilarToId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
