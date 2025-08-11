using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Footactique.Services.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingUsersUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing users to use email as username if username is empty or null
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers"" 
                SET ""UserName"" = ""Email"" 
                WHERE ""UserName"" IS NULL OR ""UserName"" = '' OR ""UserName"" = ""Email""
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
