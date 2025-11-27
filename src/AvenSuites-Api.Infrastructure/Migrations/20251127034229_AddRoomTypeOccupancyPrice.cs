using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypeOccupancyPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "VceeEdmOAtqGlSiJsimMyunknS2wkbU6lmywwxxngkc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$/2cU64A7qOgrahCFG7+QCg$Ww6XdZ/tNPi8gHaoaRP2bZgFqLs8WjaDe1khEMH9b0E");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$H4vcToe4vSBYMFI9Pzg/7A$X0hjAGYo0ATuNsZfkO+g+i5gaQBnvieX6Lj5PtcXC1E");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "t/1l71mSIkD7NClI3jpg5ntlWtlWjHDb/DT8l6WUync=");

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
        }
    }
}
