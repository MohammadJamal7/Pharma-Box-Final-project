using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class updateorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierMedications_SupplierMedicationId",
                table: "SupplierOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierOrderId",
                table: "SupplierOrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierMedicationId",
                table: "SupplierOrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierMedications_SupplierMedicationId",
                table: "SupplierOrderItems",
                column: "SupplierMedicationId",
                principalTable: "SupplierMedications",
                principalColumn: "SupplierMedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierMedications_SupplierMedicationId",
                table: "SupplierOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierOrderId",
                table: "SupplierOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SupplierMedicationId",
                table: "SupplierOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierMedications_SupplierMedicationId",
                table: "SupplierOrderItems",
                column: "SupplierMedicationId",
                principalTable: "SupplierMedications",
                principalColumn: "SupplierMedicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
