using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193") });

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "iUHVQP0T92cp/oEAmQDCnyTaUVt7RGnzi2cRNIcDXcQ=");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"),
                column: "Description",
                value: "Administrator role with full access to all hotels");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel administrator role with access to specific hotel only", true, "Hotel-Admin", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$pKVDUGmnfneyLbixU7g1uA$d+1b5V9cpdqADW9ufucbR88/YxDDB930Lg/Zylb6YL0");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$gRgdm2PJKPq530bISXiflg$m8OpAdgDDRjP4Ux83ulJWpcmHgcwxsud06l6pe3D6iQ");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "+/vCNIXj9LVGf/r+3MHpHl9tbvqrb8KD9pjv90XUyE8=");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"),
                column: "Description",
                value: "Administrator role with full access");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[] { new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3"), new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$3GTVAVhtVdcEp8P9tffgKQ$rdQQ3UieMry24VbryjGrXjqMxFz6saq4c+Zy4Hv1nqM");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$FINHXKJuQFFQFbzY1WlwmQ$2nKmeDTeh6cg3Bt9j2FiXacWATkgjo1G5xXlqo0l5dA");
        }
    }
}
