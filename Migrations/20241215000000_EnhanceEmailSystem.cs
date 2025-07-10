using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Admin.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceEmailSystem : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to Emails table
            migrationBuilder.AddColumn<DateTime>(
                name: "SentOn",
                table: "Emails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Emails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateType",
                table: "Emails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateData",
                table: "Emails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Emails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Create indexes for better performance
            migrationBuilder.CreateIndex(
                name: "IX_Emails_Status",
                table: "Emails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Category",
                table: "Emails",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Priority",
                table: "Emails",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Schedule",
                table: "Emails",
                column: "Schedule");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_CreatedOn",
                table: "Emails",
                column: "CreatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Emails_Status",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_Category",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_Priority",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_Schedule",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_CreatedOn",
                table: "Emails");

            // Drop columns
            migrationBuilder.DropColumn(
                name: "SentOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "MaxRetries",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "TemplateData",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Emails");
        }
    }
} 