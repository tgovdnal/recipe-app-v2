using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RecipeApp.Models;

public class Comment
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    [Required(ErrorMessage = "Kommentartext ist erforderlich")]
    [StringLength(1000, ErrorMessage = "Kommentar darf nicht länger als 1000 Zeichen sein")]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
