using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "HotelId1",
                table: "Users",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "CS/JwbaBnkcrQ3fIllj06LcDydW/WPNN2CXmSeqpnFo=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                columns: new[] { "HotelId1", "PasswordHash" },
                values: new object[] { null, "$argon2i$v=19$m=4096,t=2,p=2$+EyurGTivmXDu42GtNem+w$xp/S+Do2ko5jQ3xebkuPFPwsstewOqL6M7wBRhOZ7hY" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                columns: new[] { "HotelId", "HotelId1", "PasswordHash" },
                values: new object[] { new Guid("7a326969-3bf6-40d9-96dc-1aecef585000"), null, "$argon2i$v=19$m=4096,t=2,p=2$oDYIoRXyFSm8fBpLfX176A$o/c8Fg3uOXNhWXlUY4v7UCssZP3A+RMgNqUu3lNrKDs" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_HotelId1",
                table: "Users",
                column: "HotelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelId1",
                table: "Users",
                column: "HotelId1",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HotelId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HotelId1",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "iUHVQP0T92cp/oEAmQDCnyTaUVt7RGnzi2cRNIcDXcQ=");

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
                columns: new[] { "HotelId", "PasswordHash" },
                values: new object[] { null, "$argon2i$v=19$m=4096,t=2,p=2$gRgdm2PJKPq530bISXiflg$m8OpAdgDDRjP4Ux83ulJWpcmHgcwxsud06l6pe3D6iQ" });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }
    }
}
