using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

namespace RecipeApp.Services;

public class RecipeService : IRecipeService
{
    private readonly RecipeDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public RecipeService(RecipeDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<List<Recipe>> GetAllAsync(string? searchTerm = null, string? tag = null, Difficulty? difficulty = null)
    {
        var query = _context.Recipes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(r => r.Title.ToLower().Contains(searchLower) || r.Description.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagLower = tag.ToLower();
            query = query.Where(r => r.Tags.ToLower().Contains(tagLower));
        }

        if (difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == difficulty.Value);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe> CreateAsync(Recipe recipe, string? userId)
    {
        recipe.CreatedAt = DateTime.UtcNow;
        recipe.UpdatedAt = DateTime.UtcNow;
        recipe.OwnerId = userId;

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return recipe;
    }

    public async Task<Recipe> UpdateAsync(Recipe recipe, string? userId, bool isAdmin)
    {
        var existingRecipe = await _context.Recipes.FindAsync(recipe.Id);
        if (existingRecipe == null)
        {
            throw new Exception($"Rezept mit ID {recipe.Id} wurde nicht gefunden.");
        }

        if (existingRecipe.OwnerId != userId && !isAdmin)
        {
            throw new UnauthorizedAccessException("Du bist nicht berechtigt, dieses Rezept zu bearbeiten.");
        }

        // Preserve original owner
        recipe.OwnerId = existingRecipe.OwnerId;

        _context.Entry(existingRecipe).CurrentValues.SetValues(recipe);
        existingRecipe.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingRecipe;
    }

    public async Task DeleteAsync(int id, string? userId, bool isAdmin)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            if (recipe.OwnerId != userId && !isAdmin)
            {
                throw new UnauthorizedAccessException("Du bist nicht berechtigt, dieses Rezept zu löschen.");
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            // Delete image if exists
            if (!string.IsNullOrEmpty(recipe.ImageUrl))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, recipe.ImageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
        }
    }

    public async Task<string?> UploadImageAsync(IBrowserFile file)
    {
        // 5 MB max
        var maxFileSize = 5 * 1024 * 1024;
        if (file.Size > maxFileSize)
        {
            throw new Exception("Die Datei darf maximal 5MB groß sein.");
        }

        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExtensions.Contains(ext))
        {
            throw new Exception("Nur JPG, PNG oder WEBP Dateien sind erlaubt.");
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var directoryPath = Path.Combine(_environment.WebRootPath, "images", "recipes");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream(maxFileSize).CopyToAsync(stream);

        return $"/images/recipes/{fileName}";
    }
}
