using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RolesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "35ce2b29-16bd-4558-a270-71a911d9ff34", null, "Custommer", "CUSTOMER" },
                    { "40409cd5-aeaf-4f4f-b82a-0247ed7d684b", null, "Supplier", "SUPPLIER" },
                    { "7cd9d41c-54ad-459b-8fcd-af55d1933022", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35ce2b29-16bd-4558-a270-71a911d9ff34");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "40409cd5-aeaf-4f4f-b82a-0247ed7d684b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7cd9d41c-54ad-459b-8fcd-af55d1933022");
        }
    }
}
