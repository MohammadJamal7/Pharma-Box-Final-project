using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class addGroupMedicine2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_GroupMedicine_GroupMedicineId",
                table: "Medicines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMedicine",
                table: "GroupMedicine");

            migrationBuilder.RenameTable(
                name: "GroupMedicine",
                newName: "GroupMedicines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMedicines",
                table: "GroupMedicines",
                column: "GroupMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_GroupMedicines_GroupMedicineId",
                table: "Medicines",
                column: "GroupMedicineId",
                principalTable: "GroupMedicines",
                principalColumn: "GroupMedicineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_GroupMedicines_GroupMedicineId",
                table: "Medicines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMedicines",
                table: "GroupMedicines");

            migrationBuilder.RenameTable(
                name: "GroupMedicines",
                newName: "GroupMedicine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMedicine",
                table: "GroupMedicine",
                column: "GroupMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_GroupMedicine_GroupMedicineId",
                table: "Medicines",
                column: "GroupMedicineId",
                principalTable: "GroupMedicine",
                principalColumn: "GroupMedicineId");
        }
    }
}
