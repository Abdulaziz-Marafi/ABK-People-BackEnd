using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABK_People_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class new1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "VacationRequest_RequestStatus",
                table: "Requests");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "11de0e34-75ff-4068-b06c-0044fa692af2", "AQAAAAIAAYagAAAAEDiEZqOH4DTi0TKGvsw3Lt3Za9mj/sLc8J8sBHrxFGTZkxL9HlZkLrZpepm+rEcQTA==", "77f87f11-2627-43b7-a652-390018a1da7b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "610d035e-e896-487a-b010-63750aacd6d5", "AQAAAAIAAYagAAAAEHXuQfaSZ5/nNvQ7KPx2QZUD29NJz9h6IVPdwKs0DGm7d7hdDt29Lh52B5CHxGxgpA==", "c7a17018-ed01-4e18-be9f-4bd8b40df33b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "emp1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3d196299-a891-4d77-99e5-7161ef932824", "AQAAAAIAAYagAAAAEG/REok9IktQmqG3zcFlw8fFFk+TwmoahtnV+E7giFwH2qJ5vPzLlslYlwZP2DsPWQ==", "2f668cf0-4e8d-4146-a4b1-3a07ae859748" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "emp2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "449ac114-0418-4232-96e8-f9476d8ccb4f", "AQAAAAIAAYagAAAAEMLTIYYFaHmOSIf4yzxdLLon9oS4BgVjOH+RdIDUMjl/Fg09PDIciS/pv/80jjTE/w==", "9f762cfc-9e8e-4b4d-a4b3-a4a6572f9a6a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "Requests",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VacationRequest_RequestStatus",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e84a3ad3-e59e-4f40-9729-8560e3a69e50", "AQAAAAIAAYagAAAAEH3Xn04GosxL77uMLDxYL9TRTvf8yn+rn5CnTPxqo3i17DGjl4k51KYT0PcSy2PvlA==", "db19ab3a-6383-4a7d-9057-757b8a346066" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7bbe7636-18f6-4257-b97c-41b0f063dc83", "AQAAAAIAAYagAAAAEO14zXPVgnd6d1ecbbPvS2XPZbhUthyQoZo3Zr9dE7AtCxAQcmvGqL78jSvs582f3A==", "f5afff01-3236-4f0e-83a5-bf3e963efa60" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "emp1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f7d4cd34-f60c-42dd-b1ee-447632eaac46", "AQAAAAIAAYagAAAAEFyCgyDXUKX86cuNvG71effke0Crt/IePsr8RAoS3i7zVL7kyk91M+UL1pnJRrOWeg==", "063e451b-1bcc-45c2-a0b3-d62390a9d61b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "emp2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f8d86d70-8e11-4850-ae05-4b7ac896db69", "AQAAAAIAAYagAAAAEPy03TwH9uP3IUWmn5SAlGLW18YqGUWIRuwMrxUo+mV7Lyw1HPlAMziXdI2K463BNA==", "e2461175-b281-4248-add3-10b4504b5a72" });
        }
    }
}
