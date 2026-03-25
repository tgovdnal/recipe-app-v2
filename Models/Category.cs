using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeApp.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string Icon { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    [JsonIgnore]
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
