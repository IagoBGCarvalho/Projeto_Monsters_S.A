using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonstersSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class MigrationInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "TournamentDefinitions",
                columns: table => new
                {
                    TournamentDefinitionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    BuyInAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentDefinitions", x => x.TournamentDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "StatementFiles",
                columns: table => new
                {
                    StatementFileId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    UploadDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementFiles", x => x.StatementFileId);
                    table.ForeignKey(
                        name: "FK_StatementFiles_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayedTournaments",
                columns: table => new
                {
                    PlayedTournamentId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReferenceId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalBuyIn = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TotalPayout = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    NetResult = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TournamentDefinitionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayedTournaments", x => x.PlayedTournamentId);
                    table.ForeignKey(
                        name: "FK_PlayedTournaments_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayedTournaments_TournamentDefinitions_TournamentDefinitionId",
                        column: x => x.TournamentDefinitionId,
                        principalTable: "TournamentDefinitions",
                        principalColumn: "TournamentDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ReferenceId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CashAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Points = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    StatementFileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_StatementFiles_StatementFileId",
                        column: x => x.StatementFileId,
                        principalTable: "StatementFiles",
                        principalColumn: "StatementFileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TournamentDefinitions",
                columns: new[] { "TournamentDefinitionId", "BuyInAmount", "Name", "StartTime" },
                values: new object[,]
                {
                    { 1, 5.50m, "MICRO ROLLER", new TimeOnly(23, 34, 0) },
                    { 2, 5.50m, "MICRO NIGHTLY", new TimeOnly(1, 54, 0) },
                    { 3, 11.00m, "LUCKY 7s", new TimeOnly(0, 20, 0) },
                    { 4, 11.00m, "CRAZY 8s", new TimeOnly(14, 30, 0) },
                    { 5, 11.00m, "CRAZY KO", new TimeOnly(23, 10, 0) },
                    { 6, 16.50m, "LOW ROLLER", new TimeOnly(22, 34, 0) },
                    { 7, 16.50m, "CRAZY 8s MADRUGADA", new TimeOnly(2, 10, 0) },
                    { 8, 9.90m, "MINOR NINER", new TimeOnly(3, 40, 0) },
                    { 9, 9.90m, "EARLY NINER", new TimeOnly(15, 30, 0) },
                    { 10, 4.40m, "MICRO MONSTER", new TimeOnly(1, 10, 0) },
                    { 11, 55.00m, "NIGHTLY", new TimeOnly(2, 30, 0) },
                    { 12, 0.00m, "FREEROLL", new TimeOnly(0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayedTournaments_PlayerId",
                table: "PlayedTournaments",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayedTournaments_ReferenceId",
                table: "PlayedTournaments",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayedTournaments_TournamentDefinitionId",
                table: "PlayedTournaments",
                column: "TournamentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementFiles_PlayerId",
                table: "StatementFiles",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StatementFileId",
                table: "Transactions",
                column: "StatementFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayedTournaments");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TournamentDefinitions");

            migrationBuilder.DropTable(
                name: "StatementFiles");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
