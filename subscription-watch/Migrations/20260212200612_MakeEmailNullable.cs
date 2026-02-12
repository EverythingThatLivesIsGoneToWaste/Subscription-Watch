using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace subscription_watch.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmailNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                nullable: true,
                maxLength: 100,
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
