using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployees.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cats_EmployeeId",
                table: "Cats",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cats_Employees_EmployeeId",
                table: "Cats",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cats_Employees_EmployeeId",
                table: "Cats");

            migrationBuilder.DropIndex(
                name: "IX_Cats_EmployeeId",
                table: "Cats");
        }
    }
}
