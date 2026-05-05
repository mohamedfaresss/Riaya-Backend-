using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riaya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Doctors");
        }
    }
}
