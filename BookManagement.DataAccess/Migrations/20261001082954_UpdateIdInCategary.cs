using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdInCategary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryIdentification",
                table: "Categories",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryIdentification");
        }
    }
}
