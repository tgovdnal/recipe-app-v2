using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RecipeApp.Models;

public enum MealType
{
    Frühstück,
    Mittag,
    Abend,
    Snack
}

public class MealPlan
{
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public MealType MealType { get; set; }

    [Required]
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
}
