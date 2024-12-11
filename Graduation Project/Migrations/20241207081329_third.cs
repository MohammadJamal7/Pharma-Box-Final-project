using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_groupMedicines_GroupMedicineId",
                table: "Medicines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_groupMedicines",
                table: "groupMedicines");

            migrationBuilder.RenameTable(
                name: "groupMedicines",
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
                newName: "groupMedicines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_groupMedicines",
                table: "groupMedicines",
                column: "GroupMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_groupMedicines_GroupMedicineId",
                table: "Medicines",
                column: "GroupMedicineId",
                principalTable: "groupMedicines",
                principalColumn: "GroupMedicineId");
        }
    }
}
