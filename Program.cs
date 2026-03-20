using RecipeApp.Components;

using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dbPath = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=recipes.db";

// Ensure Data directory exists if running in Docker and using the default Data/recipes.db
if (dbPath.Contains("Data Source=/app/Data/"))
{
    var dir = Path.GetDirectoryName(dbPath.Replace("Data Source=", ""));
    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }
}

builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseSqlite(dbPath));

builder.Services.AddScoped<RecipeApp.Services.IRecipeService, RecipeApp.Services.RecipeService>();
builder.Services.AddHttpClient<RecipeApp.Services.IBringService, RecipeApp.Services.BringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
