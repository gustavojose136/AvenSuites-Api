using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDataBaseAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("633eb99e-dd8f-4e8d-a6ff-501b05cb2e55"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("4d5fdc8b-8944-4148-9697-61a4b5634b92"), new Guid("6c328a45-384f-4f23-8335-571980d78825") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4d5fdc8b-8944-4148-9697-61a4b5634b92"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6c328a45-384f-4f23-8335-571980d78825"));

            migrationBuilder.CreateTable(
                name: "IpmCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HotelId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CpfCnpj = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SerieNfse = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpmCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpmCredentials_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2025, 10, 27, 22, 19, 2, 756, DateTimeKind.Utc).AddTicks(9324), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2025, 10, 27, 22, 19, 2, 756, DateTimeKind.Utc).AddTicks(9486) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"), new DateTime(2025, 10, 27, 22, 19, 2, 656, DateTimeKind.Utc).AddTicks(3916), "Administrator role with full access", true, "Admin", null },
                    { new Guid("5e5da1e0-a1c3-4bf6-ad5b-c9a39e813a15"), new DateTime(2025, 10, 27, 22, 19, 2, 656, DateTimeKind.Utc).AddTicks(4268), "Standard user role", true, "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("9c6a5460-c861-4ab0-8eab-8b7dabc10de6"), new DateTime(2025, 10, 27, 22, 19, 2, 756, DateTimeKind.Utc).AddTicks(882), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$2Nnvz7sT6+Utx3T000EV2w$jkGc6uVVc6yxhtJtq+bOJPZyGZ/AUWxZuumRnv6f+Kk", null },
                    { new Guid("ab86dc1f-22ce-4e6e-9554-900d71bb3c15"), new DateTime(2025, 10, 27, 22, 19, 2, 810, DateTimeKind.Utc).AddTicks(7579), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$T+RnPWOnYmq724IYYxCj+g$V5C5MwcLnnrpnSTQaAg9YFUErSNvwgiLlqUP6tmNPiE", new DateTime(2025, 10, 27, 22, 19, 2, 810, DateTimeKind.Utc).AddTicks(7580) }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[] { new Guid("7d6d65e9-b3b9-4686-a0e9-543653a37f02"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2025, 10, 27, 22, 19, 2, 811, DateTimeKind.Utc).AddTicks(7949), "Quarto padrão com cama de casal", new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"), "Standard", new DateTime(2025, 10, 27, 22, 19, 2, 811, DateTimeKind.Utc).AddTicks(8259) });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"), new Guid("9c6a5460-c861-4ab0-8eab-8b7dabc10de6"), new DateTime(2025, 10, 27, 22, 19, 2, 756, DateTimeKind.Utc).AddTicks(3763) },
                    { new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"), new Guid("ab86dc1f-22ce-4e6e-9554-900d71bb3c15"), new DateTime(2025, 10, 27, 22, 19, 2, 811, DateTimeKind.Utc).AddTicks(349) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("22c95694-1c4a-4928-9a9a-c707c0dd91b6"), new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2465), "1", new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"), "101", new Guid("7d6d65e9-b3b9-4686-a0e9-543653a37f02"), "ACTIVE", new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2606) },
                    { new Guid("5195185c-43b0-44cf-b50b-2d634ef03236"), new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2743), "1", new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"), "103", new Guid("7d6d65e9-b3b9-4686-a0e9-543653a37f02"), "ACTIVE", new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2743) },
                    { new Guid("ace5ee94-2c9f-4836-8a53-a9e0032ef3e4"), new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2741), "1", new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"), "102", new Guid("7d6d65e9-b3b9-4686-a0e9-543653a37f02"), "ACTIVE", new DateTime(2025, 10, 27, 22, 19, 2, 812, DateTimeKind.Utc).AddTicks(2741) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IpmCredentials_HotelId",
                table: "IpmCredentials",
                column: "HotelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpmCredentials");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("5e5da1e0-a1c3-4bf6-ad5b-c9a39e813a15"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("22c95694-1c4a-4928-9a9a-c707c0dd91b6"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("5195185c-43b0-44cf-b50b-2d634ef03236"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("ace5ee94-2c9f-4836-8a53-a9e0032ef3e4"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"), new Guid("9c6a5460-c861-4ab0-8eab-8b7dabc10de6") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"), new Guid("ab86dc1f-22ce-4e6e-9554-900d71bb3c15") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2a6a15e2-3f27-42ae-aff0-742c93f1e2e9"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("7d6d65e9-b3b9-4686-a0e9-543653a37f02"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9c6a5460-c861-4ab0-8eab-8b7dabc10de6"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ab86dc1f-22ce-4e6e-9554-900d71bb3c15"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("5998a0fd-161b-438e-9269-f9cc60135ddf"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4d5fdc8b-8944-4148-9697-61a4b5634b92"), new DateTime(2025, 10, 27, 17, 21, 57, 605, DateTimeKind.Utc).AddTicks(8885), "Administrator role with full access", true, "Admin", null },
                    { new Guid("633eb99e-dd8f-4e8d-a6ff-501b05cb2e55"), new DateTime(2025, 10, 27, 17, 21, 57, 605, DateTimeKind.Utc).AddTicks(9168), "Standard user role", true, "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[] { new Guid("6c328a45-384f-4f23-8335-571980d78825"), new DateTime(2025, 10, 27, 17, 21, 57, 672, DateTimeKind.Utc).AddTicks(8470), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$HWRdCt2GqngUDNeHXAV9mg$2/1k9knaNxO5Kv9DEEXMNYgHyqPBWGmKnaZLAx/zXJ8", null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[] { new Guid("4d5fdc8b-8944-4148-9697-61a4b5634b92"), new Guid("6c328a45-384f-4f23-8335-571980d78825"), new DateTime(2025, 10, 27, 17, 21, 57, 672, DateTimeKind.Utc).AddTicks(9834) });
        }
    }
}
