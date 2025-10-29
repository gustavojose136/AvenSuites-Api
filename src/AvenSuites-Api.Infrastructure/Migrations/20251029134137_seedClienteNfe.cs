using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedClienteNfe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "GuestPii",
                columns: new[] { "GuestId", "AddressLine1", "AddressLine2", "BirthDate", "City", "CountryCode", "CreatedAt", "DocumentCipher", "DocumentKeyVersion", "DocumentNonce", "DocumentPlain", "DocumentSha256", "DocumentTag", "DocumentType", "Email", "EmailSha256", "FullName", "PhoneE164", "PhoneSha256", "PostalCode", "State", "UpdatedAt" },
                values: new object[] { new Guid("a8754c47-370d-46db-a4ed-97c900c9a96d"), "MONSENHOR GERCINO, S/N", "NÃO INFORMADO", null, "Joinville", "BR", new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(456), null, 1, null, "791.300.709-53", "c4fa5ddbd59571fba8f6bf9ab1bd9eb62d2f334bdebdb4d6be93c64fd6c478d4", null, "CPF", null, null, "Joni Cardoso", null, null, "89230-290", "SC", new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(456) });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2025, 10, 29, 13, 41, 36, 79, DateTimeKind.Utc).AddTicks(6706), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2025, 10, 29, 13, 41, 36, 79, DateTimeKind.Utc).AddTicks(6840) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"), new DateTime(2025, 10, 29, 13, 41, 35, 814, DateTimeKind.Utc).AddTicks(3202), "Administrator role with full access", true, "Admin", null },
                    { new Guid("91f3d131-c336-4cd8-a368-08a07b21307f"), new DateTime(2025, 10, 29, 13, 41, 35, 814, DateTimeKind.Utc).AddTicks(3482), "Standard user role", true, "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("983ddc9b-9ecc-448a-afdd-a7bba2e7466f"), new DateTime(2025, 10, 29, 13, 41, 36, 79, DateTimeKind.Utc).AddTicks(1382), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$P6t6bd9kr58Gx0L9X30QdA$08q1A1af3/7JqbhL8PIw4TJdFTBo5c3oBYPJBfaMykc", null },
                    { new Guid("e17b65be-8c05-42bb-bc26-2b80e04c81ad"), new DateTime(2025, 10, 29, 13, 41, 36, 122, DateTimeKind.Utc).AddTicks(6173), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$+exyIE+J1jkuZKmi/fc2Mg$qX/UiVVDkp/28+EEH2KBZX0ppBeL1yCQFXIba3rIqr8", new DateTime(2025, 10, 29, 13, 41, 36, 122, DateTimeKind.Utc).AddTicks(6177) }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "CreatedAt", "HotelId", "MarketingConsent", "UpdatedAt" },
                values: new object[] { new Guid("a8754c47-370d-46db-a4ed-97c900c9a96d"), new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(456), new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), false, new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(456) });

            migrationBuilder.InsertData(
                table: "IpmCredentials",
                columns: new[] { "Id", "Active", "CityCode", "CpfCnpj", "CreatedAt", "HotelId", "Password", "SerieNfse", "UpdatedAt", "Username" },
                values: new object[] { new Guid("d11b87fd-e5a8-4dc8-a9e5-5bc39ecf03e2"), true, "8319", "83.630.657/0001-60", new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(68), new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "pv7yBjYUCfw/FM902bBb1ity9QIC5VONEjRwt0ZX/Fw=", "1", new DateTime(2025, 10, 29, 13, 41, 36, 126, DateTimeKind.Utc).AddTicks(206), "83.630.657/0001-60" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4d260b61-c979-4f38-91a5-b5a47ff66aa1"), true, 130.00m, (short)1, (short)0, "BSC", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(1481), "Quarto básico com cama de casal", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "Basic", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(1482) },
                    { new Guid("cb948499-c81a-4714-a569-695fc4931b8a"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(1211), "Quarto padrão com cama de casal", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "Standard", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(1352) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"), new Guid("983ddc9b-9ecc-448a-afdd-a7bba2e7466f"), new DateTime(2025, 10, 29, 13, 41, 36, 79, DateTimeKind.Utc).AddTicks(2798) },
                    { new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"), new Guid("e17b65be-8c05-42bb-bc26-2b80e04c81ad"), new DateTime(2025, 10, 29, 13, 41, 36, 122, DateTimeKind.Utc).AddTicks(7598) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0e4afa29-0228-4539-a5e2-ca4ac36b87f3"), new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3389), "1", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "101", new Guid("cb948499-c81a-4714-a569-695fc4931b8a"), "ACTIVE", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3531) },
                    { new Guid("1b6384b8-ed78-4b07-98f9-700bf41bcc05"), new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3656), "1", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "102", new Guid("cb948499-c81a-4714-a569-695fc4931b8a"), "ACTIVE", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3656) },
                    { new Guid("72d6ac2b-01f7-4579-aec3-fea86a7bb65c"), new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3658), "1", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "103", new Guid("cb948499-c81a-4714-a569-695fc4931b8a"), "ACTIVE", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3658) },
                    { new Guid("d11719fa-2d7e-4b44-a8c6-ff4061207b71"), new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3660), "1", new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"), "11", new Guid("4d260b61-c979-4f38-91a5-b5a47ff66aa1"), "ACTIVE", new DateTime(2025, 10, 29, 13, 41, 36, 123, DateTimeKind.Utc).AddTicks(3660) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: new Guid("a8754c47-370d-46db-a4ed-97c900c9a96d"));

            migrationBuilder.DeleteData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("d11b87fd-e5a8-4dc8-a9e5-5bc39ecf03e2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("91f3d131-c336-4cd8-a368-08a07b21307f"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("0e4afa29-0228-4539-a5e2-ca4ac36b87f3"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("1b6384b8-ed78-4b07-98f9-700bf41bcc05"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("72d6ac2b-01f7-4579-aec3-fea86a7bb65c"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("d11719fa-2d7e-4b44-a8c6-ff4061207b71"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"), new Guid("983ddc9b-9ecc-448a-afdd-a7bba2e7466f") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"), new Guid("e17b65be-8c05-42bb-bc26-2b80e04c81ad") });

            migrationBuilder.DeleteData(
                table: "GuestPii",
                keyColumn: "GuestId",
                keyValue: new Guid("a8754c47-370d-46db-a4ed-97c900c9a96d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6a6824b5-cff6-49b2-8469-6b880644c7fc"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("4d260b61-c979-4f38-91a5-b5a47ff66aa1"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("cb948499-c81a-4714-a569-695fc4931b8a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("983ddc9b-9ecc-448a-afdd-a7bba2e7466f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e17b65be-8c05-42bb-bc26-2b80e04c81ad"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("597a3be1-5d64-48b9-a3ce-4ef6f48ce845"));

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
    }
}
