using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.API.Migrations
{
    /// <inheritdoc />
    public partial class handle_extra_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_ApplicationUserId1",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ApplicationUserId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ApplicationUserId1",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Doctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ApplicationUserId1",
                table: "Patients",
                column: "ApplicationUserId1",
                unique: true,
                filter: "[ApplicationUserId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ApplicationUserId1",
                table: "Doctors",
                column: "ApplicationUserId1",
                unique: true,
                filter: "[ApplicationUserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_ApplicationUserId1",
                table: "Doctors",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId1",
                table: "Patients",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
