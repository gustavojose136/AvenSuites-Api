using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_GuestPii_Id",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Hotels_HotelId",
                table: "Guests");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Guests",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Guests",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441"),
                columns: new[] { "UserId", "UserId1" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "t/1l71mSIkD7NClI3jpg5ntlWtlWjHDb/DT8l6WUync=");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guest role for customers who can make reservations", true, "Guest", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$ldooqsR00iYpvFt+YkUehg$0V98em3kB/VyPEBxhIw7LYTbk4tUzkSii9N80eFCKYY");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$i9lLxfYPsWLlb0+hViN87A$TiNuwkwgjRUAxQ4qlsH3qxlAtUQTSmbhaNWVIllyBuE");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_UserId",
                table: "Guests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_UserId1",
                table: "Guests",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestPii_Guests_GuestId",
                table: "GuestPii",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Hotels_HotelId",
                table: "Guests",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Users_UserId",
                table: "Guests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Users_UserId1",
                table: "Guests",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestPii_Guests_GuestId",
                table: "GuestPii");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Hotels_HotelId",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Users_UserId",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Users_UserId1",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_UserId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_UserId1",
                table: "Guests");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Guests");

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
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$+EyurGTivmXDu42GtNem+w$xp/S+Do2ko5jQ3xebkuPFPwsstewOqL6M7wBRhOZ7hY");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$oDYIoRXyFSm8fBpLfX176A$o/c8Fg3uOXNhWXlUY4v7UCssZP3A+RMgNqUu3lNrKDs");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_GuestPii_Id",
                table: "Guests",
                column: "Id",
                principalTable: "GuestPii",
                principalColumn: "GuestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Hotels_HotelId",
                table: "Guests",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
