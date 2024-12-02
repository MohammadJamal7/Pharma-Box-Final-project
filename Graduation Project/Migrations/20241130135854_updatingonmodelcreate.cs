using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class updatingonmodelcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PharmacyBranch_BranchId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PharmacyBranch_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "PharmacyBranch",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PharmacyBranch_BranchId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PharmacyBranch_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "PharmacyBranch",
                principalColumn: "BranchId");
        }
    }
}
