using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvenSuitesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRoomPricePerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomTypeOccupancyPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoomTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Occupancy = table.Column<short>(type: "smallint", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypeOccupancyPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTypeOccupancyPrices_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "IpmCredentials",
                keyColumn: "Id",
                keyValue: new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716"),
                column: "Password",
                value: "iXVIDwNjcS4yHK22i0Si7QFn8j18xmYsZHrUhohVyaY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2975cf19-0baa-4507-9f98-968760deb546"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$367MWIW+Z68rhA38rCGHVA$CCNTwa0sORwQfAS8iykiTkyj242ONjKumeTstMcZfGc");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193"),
                column: "PasswordHash",
                value: "$argon2i$v=19$m=4096,t=2,p=2$DJaHWTMiutsZYZEYlR9now$2+tE9IXvmJD2dltvL/zm4Hdh9xWUuajS0KetYUtTAKk");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeOccupancyPrices_RoomTypeId_Occupancy",
                table: "RoomTypeOccupancyPrices",
                columns: new[] { "RoomTypeId", "Occupancy" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomTypeOccupancyPrices");

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
    }
}
