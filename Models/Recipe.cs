using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

public enum Difficulty
{
    Leicht,
    Mittel,
    Schwer
}

public class Recipe
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel ist erforderlich")]
    [StringLength(100, ErrorMessage = "Titel darf nicht länger als 100 Zeichen sein")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beschreibung ist erforderlich")]
    [StringLength(500, ErrorMessage = "Beschreibung darf nicht länger als 500 Zeichen sein")]
    public string Description { get; set; } = string.Empty;

    public string IngredientsJson { get; set; } = "[]";

    public string InstructionsJson { get; set; } = "[]";

    public string Tags { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    [Range(1, 1000, ErrorMessage = "Kochzeit muss zwischen 1 und 1000 Minuten liegen")]
    public int CookingTimeMinutes { get; set; }

    [Range(1, 100, ErrorMessage = "Portionen müssen zwischen 1 und 100 liegen")]
    public int Servings { get; set; }

    public string? ImageUrl { get; set; }

    public string? OwnerId { get; set; }
    public Microsoft.AspNetCore.Identity.IdentityUser? Owner { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
