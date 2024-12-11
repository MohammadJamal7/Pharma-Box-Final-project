using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class updateMedicineModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupMedicineId",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupMedicines",
                columns: table => new
                {
                    GroupMedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMedicines", x => x.GroupMedicineId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_GroupMedicineId",
                table: "Medicines",
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

            migrationBuilder.DropTable(
                name: "GroupMedicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_GroupMedicineId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "GroupMedicineId",
                table: "Medicines");
        }
    }
}
