using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedIpmCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2025, 10, 29, 13, 20, 37, 718, DateTimeKind.Utc).AddTicks(3848), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2025, 10, 29, 13, 20, 37, 718, DateTimeKind.Utc).AddTicks(3986) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("30844855-8268-49d9-8f7c-315f6909c6d2"), new DateTime(2025, 10, 29, 13, 20, 37, 648, DateTimeKind.Utc).AddTicks(2252), "Standard user role", true, "User", null },
                    { new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"), new DateTime(2025, 10, 29, 13, 20, 37, 648, DateTimeKind.Utc).AddTicks(1974), "Administrator role with full access", true, "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6093698a-3417-419c-90b1-36330472d3a1"), new DateTime(2025, 10, 29, 13, 20, 37, 717, DateTimeKind.Utc).AddTicks(8497), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$2YH0wRtEgZcnBYoZi0qQbw$CUhhwXQAG1t3mRbUKGu35rbBU9KsKf8BmX511661dMY", null },
                    { new Guid("c3f6a64f-934b-4504-8f98-01ee1d6f8c0f"), new DateTime(2025, 10, 29, 13, 20, 37, 760, DateTimeKind.Utc).AddTicks(9795), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$f3OzmS5VSTsksN8nDSyCGQ$nd4EcMlH0KUpHeVbcJslID48XRb0Taf+Gv/8+uqyyQ4", new DateTime(2025, 10, 29, 13, 20, 37, 760, DateTimeKind.Utc).AddTicks(9797) }
                });

            migrationBuilder.InsertData(
                table: "IpmCredentials",
                columns: new[] { "Id", "Active", "CityCode", "CpfCnpj", "CreatedAt", "HotelId", "Password", "SerieNfse", "UpdatedAt", "Username" },
                values: new object[] { new Guid("794b9694-4cdc-4482-819d-c4b2da394b65"), true, "8319", "83.630.657/0001-60", new DateTime(2025, 10, 29, 13, 20, 37, 764, DateTimeKind.Utc).AddTicks(5839), new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "pb8xog1OYzj4D5yATidJ7DsSSCxXM9SHqWpBQ4k/FJk=", "1", new DateTime(2025, 10, 29, 13, 20, 37, 764, DateTimeKind.Utc).AddTicks(5979), "83.630.657/0001-60" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[] { new Guid("3a6c48fd-7207-4546-b8e7-bf57c406ea2c"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(4480), "Quarto padrão com cama de casal", new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "Standard", new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(4615) });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"), new Guid("6093698a-3417-419c-90b1-36330472d3a1"), new DateTime(2025, 10, 29, 13, 20, 37, 718, DateTimeKind.Utc).AddTicks(15) },
                    { new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"), new Guid("c3f6a64f-934b-4504-8f98-01ee1d6f8c0f"), new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(1188) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6de33575-3ecd-4d98-aec7-f395e87dff88"), new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6867), "1", new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "102", new Guid("3a6c48fd-7207-4546-b8e7-bf57c406ea2c"), "ACTIVE", new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6867) },
                    { new Guid("d4e8b24f-b31c-4246-bfbd-191d60aaba4d"), new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6869), "1", new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "103", new Guid("3a6c48fd-7207-4546-b8e7-bf57c406ea2c"), "ACTIVE", new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6869) },
                    { new Guid("f7f9c423-9326-4cc9-9093-43150ee588ef"), new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6612), "1", new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"), "101", new Guid("3a6c48fd-7207-4546-b8e7-bf57c406ea2c"), "ACTIVE", new DateTime(2025, 10, 29, 13, 20, 37, 761, DateTimeKind.Utc).AddTicks(6744) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("794b9694-4cdc-4482-819d-c4b2da394b65"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("30844855-8268-49d9-8f7c-315f6909c6d2"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("6de33575-3ecd-4d98-aec7-f395e87dff88"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("d4e8b24f-b31c-4246-bfbd-191d60aaba4d"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("f7f9c423-9326-4cc9-9093-43150ee588ef"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"), new Guid("6093698a-3417-419c-90b1-36330472d3a1") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"), new Guid("c3f6a64f-934b-4504-8f98-01ee1d6f8c0f") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a28cc8e6-87a8-4d88-a4b5-722204074350"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("3a6c48fd-7207-4546-b8e7-bf57c406ea2c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6093698a-3417-419c-90b1-36330472d3a1"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3f6a64f-934b-4504-8f98-01ee1d6f8c0f"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("7ef4134a-0954-4a37-8be5-6d39c19f3d9e"));

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
        }
    }
}
