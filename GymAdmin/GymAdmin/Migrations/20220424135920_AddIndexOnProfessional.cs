using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAdmin.Migrations
{
    public partial class AddIndexOnProfessional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Professionals_UserId",
                table: "Professionals");

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_UserId_ProfessionalType",
                table: "Professionals",
                columns: new[] { "UserId", "ProfessionalType" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Professionals_UserId_ProfessionalType",
                table: "Professionals");

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_UserId",
                table: "Professionals",
                column: "UserId");
        }
    }
}
