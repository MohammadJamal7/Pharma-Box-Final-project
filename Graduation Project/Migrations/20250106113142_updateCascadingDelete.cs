using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class updateCascadingDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_PharmacyBranch_BranchId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Inventory_InventoryId",
                table: "Medicines");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_PharmacyBranch_BranchId",
                table: "Inventory",
                column: "BranchId",
                principalTable: "PharmacyBranch",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Inventory_InventoryId",
                table: "Medicines",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_PharmacyBranch_BranchId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Inventory_InventoryId",
                table: "Medicines");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_PharmacyBranch_BranchId",
                table: "Inventory",
                column: "BranchId",
                principalTable: "PharmacyBranch",
                principalColumn: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Inventory_InventoryId",
                table: "Medicines",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "InventoryId");
        }
    }
}
