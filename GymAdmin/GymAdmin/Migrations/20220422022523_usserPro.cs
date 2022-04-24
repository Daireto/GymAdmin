using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAdmin.Migrations
{
    public partial class usserPro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_UserProfessional_userProfessionalId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfessional_Services_serviceId",
                table: "UserProfessional");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfessional",
                table: "UserProfessional");

            migrationBuilder.RenameTable(
                name: "UserProfessional",
                newName: "Professionals");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfessional_serviceId",
                table: "Professionals",
                newName: "IX_Professionals_serviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Professionals",
                table: "Professionals",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_Id",
                table: "Professionals",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Professionals_Services_serviceId",
                table: "Professionals",
                column: "serviceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Professionals_userProfessionalId",
                table: "Schedule",
                column: "userProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professionals_Services_serviceId",
                table: "Professionals");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Professionals_userProfessionalId",
                table: "Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Professionals",
                table: "Professionals");

            migrationBuilder.DropIndex(
                name: "IX_Professionals_Id",
                table: "Professionals");

            migrationBuilder.RenameTable(
                name: "Professionals",
                newName: "UserProfessional");

            migrationBuilder.RenameIndex(
                name: "IX_Professionals_serviceId",
                table: "UserProfessional",
                newName: "IX_UserProfessional_serviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfessional",
                table: "UserProfessional",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_UserProfessional_userProfessionalId",
                table: "Schedule",
                column: "userProfessionalId",
                principalTable: "UserProfessional",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfessional_Services_serviceId",
                table: "UserProfessional",
                column: "serviceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
