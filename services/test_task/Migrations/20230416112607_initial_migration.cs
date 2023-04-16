using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace test_task.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Sex = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Residents",
                columns: new[] { "Id", "Age", "Name", "Sex" },
                values: new object[,]
                {
                    { "acmwojeiwqe9", 19, "Anna Titova", "female" },
                    { "ajkvdnLdj22po11", 27, "Pishkun Vladislav", "male" },
                    { "cnoqjpIdjkqpo11", 11, "Sascha Braemer", "male" },
                    { "djkqpo11cnoqjpI", 31, "Jessica Braemer", "female" },
                    { "guyqwhoij6", 78, "Dmitry Vegas", "male" },
                    { "hqwuiehquikxhc5", 42, "German Titov", "male" },
                    { "kjlqwoijeo7", 41, "Solomon Shlemovich", "male" },
                    { "lkkpokqw8", 45, "Alex Whitedrinker", "female" },
                    { "lkqjweiojqiow4", 31, "Erzhena Koroleva", "female" },
                    { "qjIdwojqiowj10", 63, "Elmo Kennedy", "male" },
                    { "qmvqqwrqsds2", 14, "Jack Anderson", "male" },
                    { "qyfgqiyhwfoq1", 30, "Stan Smith", "male" },
                    { "vdhgqvgj3", 24, "Olga Popova", "female" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Residents");
        }
    }
}
