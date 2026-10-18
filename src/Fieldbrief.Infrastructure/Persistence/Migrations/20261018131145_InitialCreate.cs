using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fieldbrief.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Engagements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProblemStatement = table.Column<string>(type: "text", nullable: false),
                    DesiredOutcome = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Owner = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engagements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Phase = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_Engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "Engagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Kind = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsCritical = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSystems_Engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "Engagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Context = table.Column<string>(type: "text", nullable: false),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    Consequences = table.Column<string>(type: "text", nullable: false),
                    MadeBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    MadeAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FeedBackToProduct = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decisions_Engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "Engagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Author = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "Engagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerSystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpectedStatusCode = table.Column<int>(type: "integer", nullable: false),
                    TimeoutMs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthChecks_CustomerSystems_CustomerSystemId",
                        column: x => x.CustomerSystemId,
                        principalTable: "CustomerSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthCheckResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HealthCheckDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RanAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Passed = table.Column<bool>(type: "boolean", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthCheckResults_HealthChecks_HealthCheckDefinitionId",
                        column: x => x.HealthCheckDefinitionId,
                        principalTable: "HealthChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_EngagementId",
                table: "ChecklistItems",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSystems_EngagementId",
                table: "CustomerSystems",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_EngagementId",
                table: "Decisions",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_Code",
                table: "Engagements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheckResults_HealthCheckDefinitionId_RanAt",
                table: "HealthCheckResults",
                columns: new[] { "HealthCheckDefinitionId", "RanAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HealthChecks_CustomerSystemId",
                table: "HealthChecks",
                column: "CustomerSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_EngagementId",
                table: "Notes",
                column: "EngagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "Decisions");

            migrationBuilder.DropTable(
                name: "HealthCheckResults");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "HealthChecks");

            migrationBuilder.DropTable(
                name: "CustomerSystems");

            migrationBuilder.DropTable(
                name: "Engagements");
        }
    }
}
