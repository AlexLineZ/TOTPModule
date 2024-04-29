using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OTPModule.Migrations
{
    /// <inheritdoc />
    public partial class Secretkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "userEntities");

            migrationBuilder.AddColumn<Guid>(
                name: "SecretKeyId",
                table: "userEntities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "secretKeyEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretKey = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secretKeyEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userEntities_SecretKeyId",
                table: "userEntities",
                column: "SecretKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_userEntities_secretKeyEntities_SecretKeyId",
                table: "userEntities",
                column: "SecretKeyId",
                principalTable: "secretKeyEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userEntities_secretKeyEntities_SecretKeyId",
                table: "userEntities");

            migrationBuilder.DropTable(
                name: "secretKeyEntities");

            migrationBuilder.DropIndex(
                name: "IX_userEntities_SecretKeyId",
                table: "userEntities");

            migrationBuilder.DropColumn(
                name: "SecretKeyId",
                table: "userEntities");

            migrationBuilder.AddColumn<byte[]>(
                name: "SecretKey",
                table: "userEntities",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
