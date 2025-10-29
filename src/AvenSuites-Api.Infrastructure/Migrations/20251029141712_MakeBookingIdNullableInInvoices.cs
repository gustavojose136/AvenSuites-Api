using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeBookingIdNullableInInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "Invoices",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.InsertData(
                table: "GuestPii",
                columns: new[] { "GuestId", "AddressLine1", "AddressLine2", "BirthDate", "City", "CountryCode", "CreatedAt", "DocumentCipher", "DocumentKeyVersion", "DocumentNonce", "DocumentPlain", "DocumentSha256", "DocumentTag", "DocumentType", "Email", "EmailSha256", "FullName", "PhoneE164", "PhoneSha256", "PostalCode", "State", "UpdatedAt" },
                values: new object[] { new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"), "MONSENHOR GERCINO, S/N", "NÃO INFORMADO", null, "Joinville", "BR", new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2937), null, 1, null, "791.300.709-53", "c4fa5ddbd59571fba8f6bf9ab1bd9eb62d2f334bdebdb4d6be93c64fd6c478d4", null, "CPF", null, null, "Joni Cardoso", null, null, "89230-290", "SC", new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2937) });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2025, 10, 29, 14, 17, 10, 157, DateTimeKind.Utc).AddTicks(9808), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2025, 10, 29, 14, 17, 10, 157, DateTimeKind.Utc).AddTicks(9947) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("27648377-84b3-44ef-b9b0-45c9cd8fd9fc"), new DateTime(2025, 10, 29, 14, 17, 10, 79, DateTimeKind.Utc).AddTicks(3065), "Standard user role", true, "User", null },
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new DateTime(2025, 10, 29, 14, 17, 10, 79, DateTimeKind.Utc).AddTicks(2793), "Administrator role with full access", true, "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2975cf19-0baa-4507-9f98-968760deb546"), new DateTime(2025, 10, 29, 14, 17, 10, 204, DateTimeKind.Utc).AddTicks(7634), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$xhttTBbBsHOWDd79wY9C8Q$ZYpppN25AdVtMVIk1mO7QGuyLPFXVV2xo0Qf7hG0Xr8", new DateTime(2025, 10, 29, 14, 17, 10, 204, DateTimeKind.Utc).AddTicks(7637) },
                    { new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2025, 10, 29, 14, 17, 10, 157, DateTimeKind.Utc).AddTicks(4203), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$HUUHRdtkoxoh3zV2p5sRig$Gmy7cLPa7fRUofrOXrdgdaUBku9KXzGvYT4tY6LPG1Y", null }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "CreatedAt", "HotelId", "MarketingConsent", "UpdatedAt" },
                values: new object[] { new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"), new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2937), new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), false, new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2937) });

            migrationBuilder.InsertData(
                table: "IpmCredentials",
                columns: new[] { "Id", "Active", "CityCode", "CpfCnpj", "CreatedAt", "HotelId", "Password", "SerieNfse", "UpdatedAt", "Username" },
                values: new object[] { new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"), true, "8319", "83.630.657/0001-60", new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2531), new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "h7MBnnzoNN+uaBOBW9xSqg6ZoOdAI8uTHDoBVj+w3YU=", "1", new DateTime(2025, 10, 29, 14, 17, 10, 208, DateTimeKind.Utc).AddTicks(2678), "83.630.657/0001-60" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(2493), "Quarto padrão com cama de casal", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Standard", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(2630) },
                    { new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5"), true, 130.00m, (short)1, (short)0, "BSC", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(2760), "Quarto básico com cama de casal", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Basic", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(2760) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("2975cf19-0baa-4507-9f98-968760deb546"), new DateTime(2025, 10, 29, 14, 17, 10, 204, DateTimeKind.Utc).AddTicks(9093) },
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2025, 10, 29, 14, 17, 10, 157, DateTimeKind.Utc).AddTicks(5840) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("40d5718c-dbda-40c7-a4f4-644cd6f177bd"), new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4933), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "11", new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5"), "ACTIVE", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4934) },
                    { new Guid("4cdcf044-587e-4047-b164-a8cd64bad303"), new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4671), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "101", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4805) },
                    { new Guid("6bd29bd5-4826-45a0-b734-3197fec5cfbd"), new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4931), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "103", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4932) },
                    { new Guid("bd823cb6-d7a4-45ae-9853-66895ea593bb"), new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4929), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "102", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2025, 10, 29, 14, 17, 10, 205, DateTimeKind.Utc).AddTicks(4929) }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices");

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"));

            migrationBuilder.DeleteData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("27648377-84b3-44ef-b9b0-45c9cd8fd9fc"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("40d5718c-dbda-40c7-a4f4-644cd6f177bd"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("4cdcf044-587e-4047-b164-a8cd64bad303"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("6bd29bd5-4826-45a0-b734-3197fec5cfbd"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("bd823cb6-d7a4-45ae-9853-66895ea593bb"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("2975cf19-0baa-4507-9f98-968760deb546") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193") });

            migrationBuilder.DeleteData(
                table: "GuestPii",
                keyColumn: "GuestId",
                keyValue: new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "Invoices",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
