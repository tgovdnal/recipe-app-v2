using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IngredientsJson = table.Column<string>(type: "TEXT", nullable: false),
                    InstructionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    CookingTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Servings = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CookingTimeMinutes", "CreatedAt", "Description", "Difficulty", "ImageUrl", "IngredientsJson", "InstructionsJson", "Servings", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 20, new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5267), "Der italienische Klassiker mit Speck und Ei.", 1, null, "[\"400g Spaghetti\",\"150g Guanciale (oder Pancetta)\",\"4 Eier\",\"100g Pecorino Romano\",\"Schwarzer Pfeffer\"]", "[\"Nudeln in Salzwasser al dente kochen.\",\"Guanciale in einer Pfanne knusprig braten.\",\"Eier und geriebenen Pecorino verquirlen.\",\"Nudeln abtropfen lassen und zum Speck in die Pfanne geben (vom Herd nehmen!).\",\"Ei-K\\u00E4se-Mischung unterr\\u00FChren, bis eine cremige Sauce entsteht. Mit Pfeffer w\\u00FCrzen.\"]", 4, "Italienisch, Pasta, Schnell", "Spaghetti Carbonara", new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5268) },
                    { 2, 30, new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5366), "Deftige Spätzle mit reichlich Käse und Röstzwiebeln.", 0, null, "[\"500g Sp\\u00E4tzle\",\"200g Emmentaler\",\"100g Bergk\\u00E4se\",\"2 gro\\u00DFe Zwiebeln\",\"2 EL Butter\",\"Salz, Pfeffer, Muskatnuss\"]", "[\"Zwiebeln in Ringe schneiden und in Butter goldbraun r\\u00F6sten.\",\"Sp\\u00E4tzle nach Packungsanweisung kochen.\",\"K\\u00E4se reiben.\",\"Sp\\u00E4tzle und K\\u00E4se abwechselnd in eine Pfanne schichten.\",\"Schmelzen lassen, w\\u00FCrzen und mit R\\u00F6stzwiebeln garnieren.\"]", 3, "Deutsch, Vegetarisch, Deftig", "Käsespätzle", new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5367) },
                    { 3, 140, new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5733), "Klassisches ungarisches Gulasch, stundenlang geschmort.", 1, null, "[\"800g Rindfleisch (Gulasch)\",\"800g Zwiebeln\",\"2 EL Paprikapulver edels\\u00FC\\u00DF\",\"1 TL K\\u00FCmmel\",\"2 EL Tomatenmark\",\"750ml Rinderbr\\u00FChe\"]", "[\"Zwiebeln grob w\\u00FCrfeln.\",\"Fleisch scharf anbraten, aus dem Topf nehmen.\",\"Zwiebeln im Bratfett anr\\u00F6sten, Tomatenmark und Paprikapulver kurz mitr\\u00F6sten.\",\"Mit Br\\u00FChe abl\\u00F6schen, Fleisch zur\\u00FCckgeben.\",\"Mindestens 2 Stunden bei schwacher Hitze schmoren lassen. Mit Salz, Pfeffer und K\\u00FCmmel abschmecken.\"]", 4, "Fleisch, Schmoren, Hausmannskost", "Rindergulasch", new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5734) },
                    { 4, 25, new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5929), "Süße österreichische Spezialität, fluffig und karamellisiert.", 1, null, "[\"4 Eier\",\"125ml Milch\",\"100g Mehl\",\"2 EL Zucker\",\"Prise Salz\",\"3 EL Butter\",\"Puderzucker zum Best\\u00E4uben\"]", "[\"Eier trennen. Eiwei\\u00DF mit Salz steif schlagen.\",\"Eigelb mit Milch, Mehl und Zucker glatt r\\u00FChren.\",\"Eischnee vorsichtig unterheben.\",\"Butter in einer gro\\u00DFen Pfanne schmelzen, Teig eingie\\u00DFen und stocken lassen.\",\"Wenden, in mundgerechte St\\u00FCcke rei\\u00DFen und mit etwas Zucker karamellisieren.\",\"Mit Puderzucker best\\u00E4ubt servieren.\"]", 2, "Süßspeise, Österreichisch, Dessert", "Kaiserschmarrn", new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(5929) },
                    { 5, 60, new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(6014), "Eine einfache, wärmende Suppe für kalte Tage.", 0, null, "[\"250g Tellerlinsen\",\"1 Bund Suppengr\\u00FCn\",\"2 Kartoffeln\",\"1 Zwiebel\",\"1 Liter Gem\\u00FCsebr\\u00FChe\",\"2 Paar Wiener W\\u00FCrstchen\",\"1 EL Essig\"]", "[\"Zwiebel w\\u00FCrfeln, Suppengr\\u00FCn und Kartoffeln putzen und klein schneiden.\",\"Zwiebeln and\\u00FCnsten, Linsen und restliches Gem\\u00FCse zugeben.\",\"Mit Br\\u00FChe aufgie\\u00DFen und ca. 45 Minuten kochen lassen.\",\"W\\u00FCrstchen in Scheiben schneiden und in der Suppe erw\\u00E4rmen.\",\"Mit Salz, Pfeffer und etwas Essig abschmecken.\"]", 4, "Suppe, Hausmannskost, Günstig", "Linsensuppe", new DateTime(2026, 3, 20, 13, 45, 22, 301, DateTimeKind.Utc).AddTicks(6015) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
