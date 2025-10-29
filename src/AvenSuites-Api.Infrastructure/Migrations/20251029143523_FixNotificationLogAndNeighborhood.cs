using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationLogAndNeighborhood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationLogs_NotificationTemplates_TemplateKey1",
                table: "NotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_NotificationLogs_TemplateKey1",
                table: "NotificationLogs");

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: new Guid("a9c43eeb-02ff-4050-a716-449c11746f4c"));

            migrationBuilder.DeleteData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("00c294d7-d4b2-4492-8676-1c3290098d54"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b485856b-dbd6-4a53-a631-dc64f27f9b26"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("5ec1ac17-cb4b-4b9e-9c45-4c2b5ec372a8"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("8657f118-d4b9-4f4b-ac06-17796d839fee"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("b34de5b9-c0b4-4347-8c46-56325f29a93c"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("c3295944-5383-4cee-8ddd-7f1ffe9e3195"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"), new Guid("7607cb31-dcf8-4834-908a-26c05a4b6206") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"), new Guid("8661c977-dd1d-42ee-936c-c364ca8557aa") });

            migrationBuilder.DeleteData(
                table: "GuestPii",
                keyColumn: "GuestId",
                keyValue: new Guid("a9c43eeb-02ff-4050-a716-449c11746f4c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("046b2573-bed6-421a-86d1-27b2997726b9"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: new Guid("c82dd6bd-db30-4e3d-a09f-fe63b35d77d7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7607cb31-dcf8-4834-908a-26c05a4b6206"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8661c977-dd1d-42ee-936c-c364ca8557aa"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("f908b190-4dcc-4215-b751-91ceb2753615"));

            migrationBuilder.DropColumn(
                name: "TemplateKey1",
                table: "NotificationLogs");

            migrationBuilder.InsertData(
                table: "GuestPii",
                columns: new[] { "GuestId", "AddressLine1", "AddressLine2", "BirthDate", "City", "CountryCode", "CreatedAt", "DocumentCipher", "DocumentKeyVersion", "DocumentNonce", "DocumentPlain", "DocumentSha256", "DocumentTag", "DocumentType", "Email", "EmailSha256", "FullName", "Neighborhood", "PhoneE164", "PhoneSha256", "PostalCode", "State", "UpdatedAt" },
                values: new object[] { new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"), "MONSENHOR GERCINO, S/N", "NÃO INFORMADO", null, "Joinville", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, "791.300.709-53", "c4fa5ddbd59571fba8f6bf9ab1bd9eb62d2f334bdebdb4d6be93c64fd6c478d4", null, "CPF", null, null, "Joni Cardoso", "JARIVATUBA", null, null, "89230-290", "SC", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("27648377-84b3-44ef-b9b0-45c9cd8fd9fc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard user role", true, "User", null },
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator role with full access", true, "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2975cf19-0baa-4507-9f98-968760deb546"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$3GTVAVhtVdcEp8P9tffgKQ$rdQQ3UieMry24VbryjGrXjqMxFz6saq4c+Zy4Hv1nqM", null },
                    { new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$FINHXKJuQFFQFbzY1WlwmQ$2nKmeDTeh6cg3Bt9j2FiXacWATkgjo1G5xXlqo0l5dA", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "CreatedAt", "HotelId", "MarketingConsent", "UpdatedAt" },
                values: new object[] { new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "IpmCredentials",
                columns: new[] { "Id", "Active", "CityCode", "CpfCnpj", "CreatedAt", "HotelId", "Password", "SerieNfse", "UpdatedAt", "Username" },
                values: new object[] { new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"), true, "8319", "83.630.657/0001-60", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "+/vCNIXj9LVGf/r+3MHpHl9tbvqrb8KD9pjv90XUyE8=", "1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "83.630.657/0001-60" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quarto padrão com cama de casal", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Standard", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5"), true, 130.00m, (short)1, (short)0, "BSC", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quarto básico com cama de casal", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "Basic", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("2975cf19-0baa-4507-9f98-968760deb546"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("40d5718c-dbda-40c7-a4f4-644cd6f177bd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "101", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4cdcf044-587e-4047-b164-a8cd64bad303"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "102", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6bd29bd5-4826-45a0-b734-3197fec5cfbd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "103", new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441"), "ACTIVE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bd823cb6-d7a4-45ae-9853-66895ea593bb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1", new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), "11", new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5"), "ACTIVE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_TemplateKey",
                table: "NotificationLogs",
                column: "TemplateKey");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationLogs_NotificationTemplates_TemplateKey",
                table: "NotificationLogs",
                column: "TemplateKey",
                principalTable: "NotificationTemplates",
                principalColumn: "TemplateKey",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationLogs_NotificationTemplates_TemplateKey",
                table: "NotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_NotificationLogs_TemplateKey",
                table: "NotificationLogs");

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

            migrationBuilder.AddColumn<string>(
                name: "TemplateKey1",
                table: "NotificationLogs",
                type: "varchar(80)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "GuestPii",
                columns: new[] { "GuestId", "AddressLine1", "AddressLine2", "BirthDate", "City", "CountryCode", "CreatedAt", "DocumentCipher", "DocumentKeyVersion", "DocumentNonce", "DocumentPlain", "DocumentSha256", "DocumentTag", "DocumentType", "Email", "EmailSha256", "FullName", "Neighborhood", "PhoneE164", "PhoneSha256", "PostalCode", "State", "UpdatedAt" },
                values: new object[] { new Guid("a9c43eeb-02ff-4050-a716-449c11746f4c"), "MONSENHOR GERCINO, S/N", "NÃO INFORMADO", null, "Joinville", "BR", new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4955), null, 1, null, "791.300.709-53", "c4fa5ddbd59571fba8f6bf9ab1bd9eb62d2f334bdebdb4d6be93c64fd6c478d4", null, "CPF", null, null, "Joni Cardoso", "JARIVATUBA", null, null, "89230-290", "SC", new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4955) });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Cnpj", "CountryCode", "CreatedAt", "Email", "Name", "PhoneE164", "PostalCode", "State", "Status", "Timezone", "TradeName", "UpdatedAt" },
                values: new object[] { new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "Av. Dr. Nereu Ramos, 474", "Rocio Grande, São Francisco do Sul - SC", "São Francisco do Sul", "83.630.657/0001-60", "BR", new DateTime(2025, 10, 29, 14, 31, 47, 564, DateTimeKind.Utc).AddTicks(4288), "gjose2980@gmail.com", "Hotel Avenida", "+554799662998", "89331-260", "SC", "ACTIVE", "America/Sao_Paulo", "Hotel Avenida", new DateTime(2025, 10, 29, 14, 31, 47, 564, DateTimeKind.Utc).AddTicks(4428) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"), new DateTime(2025, 10, 29, 14, 31, 47, 496, DateTimeKind.Utc).AddTicks(6918), "Administrator role with full access", true, "Admin", null },
                    { new Guid("b485856b-dbd6-4a53-a631-dc64f27f9b26"), new DateTime(2025, 10, 29, 14, 31, 47, 496, DateTimeKind.Utc).AddTicks(7192), "Standard user role", true, "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "HotelId", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("7607cb31-dcf8-4834-908a-26c05a4b6206"), new DateTime(2025, 10, 29, 14, 31, 47, 563, DateTimeKind.Utc).AddTicks(8957), "admin@avensuites.com", null, true, "Administrator", "$argon2i$v=19$m=4096,t=2,p=2$nKYnxHrTBEeGvGfJEIKGgw$dj61rpDShUP8ZjdG2FNga+JEOX9jgi8SB4YoXV85CCs", null },
                    { new Guid("8661c977-dd1d-42ee-936c-c364ca8557aa"), new DateTime(2025, 10, 29, 14, 31, 47, 605, DateTimeKind.Utc).AddTicks(9002), "gjose2980@gmail.com", null, true, "Gustavo", "$argon2i$v=19$m=4096,t=2,p=2$x1Ttnn9LEAkdkkfC04NyDg$9CTzmq9MW9EHbKsFbAynBK1Yj2eAN5Zm5YJtFKmcbwM", new DateTime(2025, 10, 29, 14, 31, 47, 605, DateTimeKind.Utc).AddTicks(9004) }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "CreatedAt", "HotelId", "MarketingConsent", "UpdatedAt" },
                values: new object[] { new Guid("a9c43eeb-02ff-4050-a716-449c11746f4c"), new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4955), new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), false, new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4955) });

            migrationBuilder.InsertData(
                table: "IpmCredentials",
                columns: new[] { "Id", "Active", "CityCode", "CpfCnpj", "CreatedAt", "HotelId", "Password", "SerieNfse", "UpdatedAt", "Username" },
                values: new object[] { new Guid("00c294d7-d4b2-4492-8676-1c3290098d54"), true, "8319", "83.630.657/0001-60", new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4570), new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "hzuBqBWGZJeNaAF27Bv+xWnr5d98UyLxx7VlHCWEd6U=", "1", new DateTime(2025, 10, 29, 14, 31, 47, 609, DateTimeKind.Utc).AddTicks(4711), "83.630.657/0001-60" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Active", "BasePrice", "CapacityAdults", "CapacityChildren", "Code", "CreatedAt", "Description", "HotelId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("046b2573-bed6-421a-86d1-27b2997726b9"), true, 150.00m, (short)2, (short)1, "STD", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(4268), "Quarto padrão com cama de casal", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "Standard", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(4426) },
                    { new Guid("c82dd6bd-db30-4e3d-a09f-fe63b35d77d7"), true, 130.00m, (short)1, (short)0, "BSC", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(4580), "Quarto básico com cama de casal", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "Basic", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(4581) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"), new Guid("7607cb31-dcf8-4834-908a-26c05a4b6206"), new DateTime(2025, 10, 29, 14, 31, 47, 564, DateTimeKind.Utc).AddTicks(457) },
                    { new Guid("215e60bd-0290-4ece-bb47-61ce219c9758"), new Guid("8661c977-dd1d-42ee-936c-c364ca8557aa"), new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(381) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CreatedAt", "Floor", "HotelId", "RoomNumber", "RoomTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5ec1ac17-cb4b-4b9e-9c45-4c2b5ec372a8"), new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7348), "1", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "11", new Guid("c82dd6bd-db30-4e3d-a09f-fe63b35d77d7"), "ACTIVE", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7349) },
                    { new Guid("8657f118-d4b9-4f4b-ac06-17796d839fee"), new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7343), "1", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "102", new Guid("046b2573-bed6-421a-86d1-27b2997726b9"), "ACTIVE", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7344) },
                    { new Guid("b34de5b9-c0b4-4347-8c46-56325f29a93c"), new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7346), "1", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "103", new Guid("046b2573-bed6-421a-86d1-27b2997726b9"), "ACTIVE", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7346) },
                    { new Guid("c3295944-5383-4cee-8ddd-7f1ffe9e3195"), new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(6998), "1", new Guid("f908b190-4dcc-4215-b751-91ceb2753615"), "101", new Guid("046b2573-bed6-421a-86d1-27b2997726b9"), "ACTIVE", new DateTime(2025, 10, 29, 14, 31, 47, 606, DateTimeKind.Utc).AddTicks(7194) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_TemplateKey1",
                table: "NotificationLogs",
                column: "TemplateKey1");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationLogs_NotificationTemplates_TemplateKey1",
                table: "NotificationLogs",
                column: "TemplateKey1",
                principalTable: "NotificationTemplates",
                principalColumn: "TemplateKey");
        }
    }
}
