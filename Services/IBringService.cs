namespace RecipeApp.Services;

public interface IBringService
{
    Task<bool> AddItemsAsync(IEnumerable<string> items);
}
