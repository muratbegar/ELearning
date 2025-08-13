using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELearningIskoop.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mg_004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                schema: "courses",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseCategories_Categories_CategoryId",
                schema: "courses",
                table: "CourseCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                schema: "courses",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "courses",
                newName: "categories",
                newSchema: "courses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                schema: "courses",
                table: "categories",
                column: "ObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                schema: "courses",
                table: "categories",
                column: "ParentCategoryId",
                principalSchema: "courses",
                principalTable: "categories",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCategories_categories_CategoryId",
                schema: "courses",
                table: "CourseCategories",
                column: "CategoryId",
                principalSchema: "courses",
                principalTable: "categories",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                schema: "courses",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseCategories_categories_CategoryId",
                schema: "courses",
                table: "CourseCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                schema: "courses",
                table: "categories");

            migrationBuilder.RenameTable(
                name: "categories",
                schema: "courses",
                newName: "Categories",
                newSchema: "courses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                schema: "courses",
                table: "Categories",
                column: "ObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                schema: "courses",
                table: "Categories",
                column: "ParentCategoryId",
                principalSchema: "courses",
                principalTable: "Categories",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCategories_Categories_CategoryId",
                schema: "courses",
                table: "CourseCategories",
                column: "CategoryId",
                principalSchema: "courses",
                principalTable: "Categories",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
