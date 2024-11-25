using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultadminseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FullName", "Password", "Role" },
                values: new object[] { 1, new DateTime(2005, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "ariellazaro444@gmail.com", "Ariel David Lázaro Pérez", "ariel123", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
