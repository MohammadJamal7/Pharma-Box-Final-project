using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class changesuppliermedicationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_SupplierMedications_SupplierMedicationId",
                table: "Medicines");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierMedicationId",
                table: "Medicines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_SupplierMedications_SupplierMedicationId",
                table: "Medicines",
                column: "SupplierMedicationId",
                principalTable: "SupplierMedications",
                principalColumn: "SupplierMedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_SupplierMedications_SupplierMedicationId",
                table: "Medicines");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierMedicationId",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_SupplierMedications_SupplierMedicationId",
                table: "Medicines",
                column: "SupplierMedicationId",
                principalTable: "SupplierMedications",
                principalColumn: "SupplierMedicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
