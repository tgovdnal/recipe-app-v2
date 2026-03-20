using Microsoft.AspNetCore.Components.Forms;
using RecipeApp.Models;

namespace RecipeApp.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetAllAsync(string? searchTerm = null, string? tag = null, Difficulty? difficulty = null);
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe> CreateAsync(Recipe recipe);
    Task<Recipe> UpdateAsync(Recipe recipe);
    Task DeleteAsync(int id);
    Task<string?> UploadImageAsync(IBrowserFile file);
}
