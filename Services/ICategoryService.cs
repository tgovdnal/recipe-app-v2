using RecipeApp.Models;

namespace RecipeApp.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
}
