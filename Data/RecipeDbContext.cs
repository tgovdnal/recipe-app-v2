using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;

namespace RecipeApp.Data;

public class RecipeDbContext : IdentityDbContext<IdentityUser>
{
    public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MealPlan> MealPlans { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-Many Recipe <-> Category
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Categories)
            .WithMany(c => c.Recipes)
            .UsingEntity("RecipeCategory");

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Vorspeisen", Icon = "🥗", SortOrder = 1 },
            new Category { Id = 2, Name = "Hauptgerichte", Icon = "🍽️", SortOrder = 2 },
            new Category { Id = 3, Name = "Desserts", Icon = "🍨", SortOrder = 3 },
            new Category { Id = 4, Name = "Salate", Icon = "🥬", SortOrder = 4 },
            new Category { Id = 5, Name = "Suppen", Icon = "🥣", SortOrder = 5 },
            new Category { Id = 6, Name = "Snacks", Icon = "🥨", SortOrder = 6 },
            new Category { Id = 7, Name = "Getränke", Icon = "🥤", SortOrder = 7 },
            new Category { Id = 8, Name = "Backen", Icon = "🍰", SortOrder = 8 }
        };
        modelBuilder.Entity<Category>().HasData(categories);

        var recipes = new List<Recipe>
        {
            new Recipe
            {
                Id = 1,
                Title = "Spaghetti Carbonara",
                Description = "Der italienische Klassiker mit Speck und Ei.",
                IngredientsJson = JsonSerializer.Serialize(new List<string> { "400g Spaghetti", "150g Guanciale (oder Pancetta)", "4 Eier", "100g Pecorino Romano", "Schwarzer Pfeffer" }),
                InstructionsJson = JsonSerializer.Serialize(new List<string> { "Nudeln in Salzwasser al dente kochen.", "Guanciale in einer Pfanne knusprig braten.", "Eier und geriebenen Pecorino verquirlen.", "Nudeln abtropfen lassen und zum Speck in die Pfanne geben (vom Herd nehmen!).", "Ei-Käse-Mischung unterrühren, bis eine cremige Sauce entsteht. Mit Pfeffer würzen." }),
                Tags = "Italienisch, Pasta, Schnell",
                Difficulty = Difficulty.Mittel,
                CookingTimeMinutes = 20,
                Servings = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Recipe
            {
                Id = 2,
                Title = "Käsespätzle",
                Description = "Deftige Spätzle mit reichlich Käse und Röstzwiebeln.",
                IngredientsJson = JsonSerializer.Serialize(new List<string> { "500g Spätzle", "200g Emmentaler", "100g Bergkäse", "2 große Zwiebeln", "2 EL Butter", "Salz, Pfeffer, Muskatnuss" }),
                InstructionsJson = JsonSerializer.Serialize(new List<string> { "Zwiebeln in Ringe schneiden und in Butter goldbraun rösten.", "Spätzle nach Packungsanweisung kochen.", "Käse reiben.", "Spätzle und Käse abwechselnd in eine Pfanne schichten.", "Schmelzen lassen, würzen und mit Röstzwiebeln garnieren." }),
                Tags = "Deutsch, Vegetarisch, Deftig",
                Difficulty = Difficulty.Leicht,
                CookingTimeMinutes = 30,
                Servings = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Recipe
            {
                Id = 3,
                Title = "Rindergulasch",
                Description = "Klassisches ungarisches Gulasch, stundenlang geschmort.",
                IngredientsJson = JsonSerializer.Serialize(new List<string> { "800g Rindfleisch (Gulasch)", "800g Zwiebeln", "2 EL Paprikapulver edelsüß", "1 TL Kümmel", "2 EL Tomatenmark", "750ml Rinderbrühe" }),
                InstructionsJson = JsonSerializer.Serialize(new List<string> { "Zwiebeln grob würfeln.", "Fleisch scharf anbraten, aus dem Topf nehmen.", "Zwiebeln im Bratfett anrösten, Tomatenmark und Paprikapulver kurz mitrösten.", "Mit Brühe ablöschen, Fleisch zurückgeben.", "Mindestens 2 Stunden bei schwacher Hitze schmoren lassen. Mit Salz, Pfeffer und Kümmel abschmecken." }),
                Tags = "Fleisch, Schmoren, Hausmannskost",
                Difficulty = Difficulty.Mittel,
                CookingTimeMinutes = 140,
                Servings = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Recipe
            {
                Id = 4,
                Title = "Kaiserschmarrn",
                Description = "Süße österreichische Spezialität, fluffig und karamellisiert.",
                IngredientsJson = JsonSerializer.Serialize(new List<string> { "4 Eier", "125ml Milch", "100g Mehl", "2 EL Zucker", "Prise Salz", "3 EL Butter", "Puderzucker zum Bestäuben" }),
                InstructionsJson = JsonSerializer.Serialize(new List<string> { "Eier trennen. Eiweiß mit Salz steif schlagen.", "Eigelb mit Milch, Mehl und Zucker glatt rühren.", "Eischnee vorsichtig unterheben.", "Butter in einer großen Pfanne schmelzen, Teig eingießen und stocken lassen.", "Wenden, in mundgerechte Stücke reißen und mit etwas Zucker karamellisieren.", "Mit Puderzucker bestäubt servieren." }),
                Tags = "Süßspeise, Österreichisch, Dessert",
                Difficulty = Difficulty.Mittel,
                CookingTimeMinutes = 25,
                Servings = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Recipe
            {
                Id = 5,
                Title = "Linsensuppe",
                Description = "Eine einfache, wärmende Suppe für kalte Tage.",
                IngredientsJson = JsonSerializer.Serialize(new List<string> { "250g Tellerlinsen", "1 Bund Suppengrün", "2 Kartoffeln", "1 Zwiebel", "1 Liter Gemüsebrühe", "2 Paar Wiener Würstchen", "1 EL Essig" }),
                InstructionsJson = JsonSerializer.Serialize(new List<string> { "Zwiebel würfeln, Suppengrün und Kartoffeln putzen und klein schneiden.", "Zwiebeln andünsten, Linsen und restliches Gemüse zugeben.", "Mit Brühe aufgießen und ca. 45 Minuten kochen lassen.", "Würstchen in Scheiben schneiden und in der Suppe erwärmen.", "Mit Salz, Pfeffer und etwas Essig abschmecken." }),
                Tags = "Suppe, Hausmannskost, Günstig",
                Difficulty = Difficulty.Leicht,
                CookingTimeMinutes = 60,
                Servings = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Recipe>().HasData(recipes);
    }
}
