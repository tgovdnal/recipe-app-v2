using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

namespace RecipeApp.Services;

public class MealPlanService : IMealPlanService
{
    private readonly RecipeDbContext _context;

    public MealPlanService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<List<MealPlan>> GetMealPlansForWeekAsync(string userId, DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(7);
        return await _context.MealPlans
            .Include(m => m.Recipe)
            .Where(m => m.UserId == userId && m.Date >= weekStart && m.Date < weekEnd)
            .ToListAsync();
    }

    public async Task<MealPlan> AssignRecipeToMealAsync(string userId, DateTime date, MealType mealType, int recipeId)
    {
        var existing = await _context.MealPlans.FirstOrDefaultAsync(m => m.UserId == userId && m.Date.Date == date.Date && m.MealType == mealType);

        if (existing != null)
        {
            existing.RecipeId = recipeId;
            await _context.SaveChangesAsync();
            return existing;
        }
        else
        {
            var mealPlan = new MealPlan
            {
                UserId = userId,
                Date = date.Date,
                MealType = mealType,
                RecipeId = recipeId
            };
            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();
            return mealPlan;
        }
    }

    public async Task RemoveRecipeFromMealAsync(string userId, int mealPlanId)
    {
        var mealPlan = await _context.MealPlans.FirstOrDefaultAsync(m => m.Id == mealPlanId && m.UserId == userId);
        if (mealPlan != null)
        {
            _context.MealPlans.Remove(mealPlan);
            await _context.SaveChangesAsync();
        }
    }
}
