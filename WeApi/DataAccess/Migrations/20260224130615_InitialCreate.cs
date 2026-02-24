using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PointsRemaining = table.Column<int>(type: "int", nullable: false),
                    PointsReceived = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyAllocations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointAssignments_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointAssignments_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Desarrollo", "maria.garcia@grupoprominente.com", "María García", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marketing", "juan.perez@grupoprominente.com", "Juan Pérez", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "People & Culture", "ana.rodriguez@grupoprominente.com", "Ana Rodríguez", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "people" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ventas", "carlos.lopez@grupoprominente.com", "Carlos López", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Desarrollo", "laura.martinez@grupoprominente.com", "Laura Martínez", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Operaciones", "diego.sanchez@grupoprominente.com", "Diego Sánchez", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marketing", "sofia.torres@grupoprominente.com", "Sofia Torres", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ventas", "roberto.flores@grupoprominente.com", "Roberto Flores", "$2a$11$hyOlTRGeP6zgPX2cZ4WwVOayoO9WOg/E.0hDp.l03bVeqKs4sNhGa", "employee" }
                });

            migrationBuilder.InsertData(
                table: "MonthlyAllocations",
                columns: new[] { "Id", "Month", "PointsReceived", "PointsRemaining", "UserId" },
                values: new object[,]
                {
                    { 1, "2026-02", 5, 5, 1 },
                    { 2, "2026-02", 4, 6, 2 },
                    { 3, "2026-02", 0, 10, 3 },
                    { 4, "2026-02", 2, 8, 4 },
                    { 5, "2026-02", 0, 6, 5 },
                    { 6, "2026-02", 0, 10, 6 },
                    { 7, "2026-02", 0, 10, 7 },
                    { 8, "2026-02", 0, 10, 8 }
                });

            migrationBuilder.InsertData(
                table: "PointAssignments",
                columns: new[] { "Id", "Category", "FromUserId", "Message", "Month", "Points", "Timestamp", "ToUserId" },
                values: new object[,]
                {
                    { 1, "Trabajo en equipo", 2, "¡Excelente colaboración en el proyecto!", "2026-02", 3, 1740268800000L, 1 },
                    { 2, "Innovación", 4, "Gran idea para mejorar el proceso", "2026-02", 2, 1739836800000L, 1 },
                    { 3, "Liderazgo", 5, null, "2026-02", 4, 1740096000000L, 2 },
                    { 4, "Colaboración", 1, "Siempre dispuesto a ayudar", "2026-02", 2, 1740355200000L, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAllocations_UserId_Month",
                table: "MonthlyAllocations",
                columns: new[] { "UserId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointAssignments_FromUserId",
                table: "PointAssignments",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PointAssignments_ToUserId",
                table: "PointAssignments",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyAllocations");

            migrationBuilder.DropTable(
                name: "PointAssignments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
