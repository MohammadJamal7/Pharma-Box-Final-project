using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSupplierOrderId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId1",
                table: "SupplierOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOrderItems_SupplierOrderId1",
                table: "SupplierOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SupplierOrderId1",
                table: "SupplierOrderItems");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierOrderId",
                table: "SupplierOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "SupplierOrderId1",
                table: "SupplierOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrderItems_SupplierOrderId1",
                table: "SupplierOrderItems",
                column: "SupplierOrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId1",
                table: "OrderItems",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                table: "OrderItems",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId",
                table: "SupplierOrderItems",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrderItems_SupplierOrders_SupplierOrderId1",
                table: "SupplierOrderItems",
                column: "SupplierOrderId1",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
