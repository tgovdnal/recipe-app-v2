using Microsoft.AspNetCore.Components.Forms;
using RecipeApp.Models;

namespace RecipeApp.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetAllAsync(string? searchTerm = null, string? tag = null, Difficulty? difficulty = null);
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe> CreateAsync(Recipe recipe, string? userId);
    Task<Recipe> UpdateAsync(Recipe recipe, string? userId, bool isAdmin);
    Task DeleteAsync(int id, string? userId, bool isAdmin);
    Task<string?> UploadImageAsync(IBrowserFile file);
}
