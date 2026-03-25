using RecipeApp.Models;

namespace RecipeApp.Services;

public interface IMealPlanService
{
    Task<List<MealPlan>> GetMealPlansForWeekAsync(string userId, DateTime weekStart);
    Task<MealPlan> AssignRecipeToMealAsync(string userId, DateTime date, MealType mealType, int recipeId);
    Task RemoveRecipeFromMealAsync(string userId, int mealPlanId);
}
