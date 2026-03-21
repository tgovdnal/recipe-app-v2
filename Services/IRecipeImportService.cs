using RecipeApp.Models;

namespace RecipeApp.Services;

public interface IRecipeImportService
{
    Task<Recipe> ImportAsync(string url);
}
