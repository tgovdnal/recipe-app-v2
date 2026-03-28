using Microsoft.AspNetCore.Components.Forms;
using RecipeApp.Models;

namespace RecipeApp.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetAllAsync(string? searchTerm = null, string? tag = null, Difficulty? difficulty = null, int? categoryId = null);
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe> CreateAsync(Recipe recipe, List<int> categoryIds, string? userId);
    Task<Recipe> UpdateAsync(Recipe recipe, List<int> categoryIds, string? userId, bool isAdmin);
    Task DeleteAsync(int id, string? userId, bool isAdmin);
    Task<string?> UploadImageAsync(IBrowserFile file);
}
