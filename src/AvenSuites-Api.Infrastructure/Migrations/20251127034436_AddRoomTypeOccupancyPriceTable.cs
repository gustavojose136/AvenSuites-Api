using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypeOccupancyPriceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "rqvXQIuaDRQlGPZZg4wxHrqntjR6JPbsK2d4MiRCPyI=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$0hOe0imo+2aNKxzp+cSbrA$UhNNj0fJH0HUvwZuHPP0BdRCQNSYGAWZV4MZG6+PC1A");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$pue9452dqAYn75p66LZ4tg$W3//vxKncXM7mv3WguJ/0dQrPTLWeO4TfPmzDkZKRH0");
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
