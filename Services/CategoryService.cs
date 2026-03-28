using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

namespace RecipeApp.Services;

public class CategoryService : ICategoryService
{
    private readonly RecipeDbContext _context;

    public CategoryService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories.OrderBy(c => c.SortOrder).ToListAsync();
    }
}
