using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RecipeApp.Models;

public class Rating
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    [Range(1, 5, ErrorMessage = "Bewertung muss zwischen 1 und 5 Sternen liegen")]
    public int Score { get; set; }

    public DateTime CreatedAt { get; set; }
}
