using System.Text.Json;
using HtmlAgilityPack;
using RecipeApp.Models;

namespace RecipeApp.Services;

public class RecipeImportService : IRecipeImportService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecipeImportService> _logger;

    public RecipeImportService(HttpClient httpClient, ILogger<RecipeImportService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<Recipe> ImportAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || (!url.StartsWith("http://") && !url.StartsWith("https://")))
        {
            throw new ArgumentException("Nur HTTP oder HTTPS URLs sind erlaubt.");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Ungültige URL.");
        }

        if (uri.IsLoopback)
        {
            throw new ArgumentException("Lokale Adressen sind nicht erlaubt.");
        }

        try
        {
            var addresses = await System.Net.Dns.GetHostAddressesAsync(uri.Host);
            foreach (var ip in addresses)
            {
                if (System.Net.IPAddress.IsLoopback(ip) || IsPrivateIpAddress(ip))
                {
                    throw new ArgumentException("Lokale oder private Adressen sind nicht erlaubt.");
                }
            }
        }
        catch (System.Net.Sockets.SocketException)
        {
             throw new Exception("Fehler beim Auflösen des Hostnamens.");
        }

        try
        {
            var html = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var recipe = ExtractFromJsonLd(doc);
            if (recipe != null)
            {
                return recipe;
            }

            recipe = ExtractFromOpenGraph(doc);
            if (recipe != null)
            {
                return recipe;
            }

            throw new Exception("Keine Rezeptdaten unter der angegebenen URL gefunden.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der URL {Url}", url);
            throw new Exception("Fehler beim Abrufen der URL. Bitte überprüfen Sie die Adresse.");
        }
        catch (TaskCanceledException ex)
        {
             _logger.LogError(ex, "Timeout beim Abrufen der URL {Url}", url);
             throw new Exception("Zeitüberschreitung beim Abrufen der URL.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unerwarteter Fehler beim Importieren der URL {Url}", url);
            throw new Exception($"Fehler beim Importieren: {ex.Message}");
        }
    }

    private Recipe? ExtractFromJsonLd(HtmlDocument doc)
    {
        var scripts = doc.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
        if (scripts == null) return null;

        foreach (var script in scripts)
        {
            try
            {
                var json = script.InnerText;
                var jsonDoc = JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;

                // Handle both single object and array of objects in JSON-LD
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in root.EnumerateArray())
                    {
                        var recipe = TryParseRecipeJson(item);
                        if (recipe != null) return recipe;
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    // Check if it's a Graph
                    if (root.TryGetProperty("@graph", out var graph) && graph.ValueKind == JsonValueKind.Array)
                    {
                         foreach (var item in graph.EnumerateArray())
                         {
                             var recipe = TryParseRecipeJson(item);
                             if (recipe != null) return recipe;
                         }
                    }
                    else
                    {
                        var recipe = TryParseRecipeJson(root);
                        if (recipe != null) return recipe;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fehler beim Parsen von JSON-LD");
            }
        }

        return null;
    }

    private Recipe? TryParseRecipeJson(JsonElement element)
    {
        if (element.TryGetProperty("@type", out var typeProp))
        {
            var type = typeProp.ValueKind == JsonValueKind.Array
                ? typeProp.EnumerateArray().FirstOrDefault().GetString()
                : typeProp.GetString();

            if (type == "Recipe")
            {
                var recipe = new Recipe
                {
                    Title = GetStringProperty(element, "name") ?? "Unbekanntes Rezept",
                    Description = GetStringProperty(element, "description") ?? "Keine Beschreibung verfügbar.",
                    Difficulty = Difficulty.Mittel,
                    Tags = GetStringProperty(element, "recipeCategory") ?? ""
                };

                // Ingredients
                if (element.TryGetProperty("recipeIngredient", out var ingredientsProp) && ingredientsProp.ValueKind == JsonValueKind.Array)
                {
                    var ingredients = ingredientsProp.EnumerateArray().Select(i => i.GetString()).Where(i => i != null).ToList();
                    recipe.IngredientsJson = JsonSerializer.Serialize(ingredients);
                }

                // Instructions
                if (element.TryGetProperty("recipeInstructions", out var instructionsProp))
                {
                    var instructions = new List<string>();
                    if (instructionsProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var inst in instructionsProp.EnumerateArray())
                        {
                            if (inst.ValueKind == JsonValueKind.Object && inst.TryGetProperty("text", out var textProp))
                            {
                                instructions.Add(textProp.GetString()!);
                            }
                            else if (inst.ValueKind == JsonValueKind.String)
                            {
                                instructions.Add(inst.GetString()!);
                            }
                        }
                    }
                    else if (instructionsProp.ValueKind == JsonValueKind.String)
                    {
                        instructions.Add(instructionsProp.GetString()!);
                    }
                    recipe.InstructionsJson = JsonSerializer.Serialize(instructions);
                }

                // Image
                if (element.TryGetProperty("image", out var imageProp))
                {
                    if (imageProp.ValueKind == JsonValueKind.Array && imageProp.GetArrayLength() > 0)
                    {
                         recipe.ImageUrl = imageProp[0].GetString();
                    }
                    else if (imageProp.ValueKind == JsonValueKind.String)
                    {
                        recipe.ImageUrl = imageProp.GetString();
                    }
                    else if (imageProp.ValueKind == JsonValueKind.Object && imageProp.TryGetProperty("url", out var urlProp))
                    {
                         recipe.ImageUrl = urlProp.GetString();
                    }
                }

                // Yield / Servings
                if (element.TryGetProperty("recipeYield", out var yieldProp))
                {
                     var yieldStr = yieldProp.ValueKind == JsonValueKind.Array ? yieldProp[0].GetString() : yieldProp.GetString();
                     if (int.TryParse(new string(yieldStr?.Where(char.IsDigit).ToArray()), out int yieldValue) && yieldValue > 0)
                     {
                         recipe.Servings = yieldValue;
                     }
                     else
                     {
                         recipe.Servings = 4;
                     }
                }
                else
                {
                    recipe.Servings = 4;
                }

                // Time
                int totalMinutes = 0;
                if (element.TryGetProperty("prepTime", out var prepTimeProp))
                {
                    totalMinutes += ParseIso8601Duration(prepTimeProp.GetString());
                }
                if (element.TryGetProperty("cookTime", out var cookTimeProp))
                {
                    totalMinutes += ParseIso8601Duration(cookTimeProp.GetString());
                }

                if (totalMinutes > 0)
                {
                     recipe.CookingTimeMinutes = totalMinutes;
                }
                else if (element.TryGetProperty("totalTime", out var totalTimeProp))
                {
                     recipe.CookingTimeMinutes = ParseIso8601Duration(totalTimeProp.GetString());
                }

                if (recipe.CookingTimeMinutes == 0) recipe.CookingTimeMinutes = 30; // fallback

                return recipe;
            }
        }
        return null;
    }

    private string? GetStringProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop))
        {
            return prop.ValueKind == JsonValueKind.Array ? prop[0].GetString() : prop.GetString();
        }
        return null;
    }

    private int ParseIso8601Duration(string? duration)
    {
        if (string.IsNullOrEmpty(duration)) return 0;
        try
        {
             return (int)System.Xml.XmlConvert.ToTimeSpan(duration).TotalMinutes;
        }
        catch
        {
             return 0;
        }
    }

    private Recipe? ExtractFromOpenGraph(HtmlDocument doc)
    {
        var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
        var descNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
        var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");

        if (titleNode != null)
        {
            return new Recipe
            {
                Title = titleNode.GetAttributeValue("content", "Unbekanntes Rezept"),
                Description = descNode?.GetAttributeValue("content", "Keine Beschreibung gefunden.") ?? "Keine Beschreibung gefunden.",
                ImageUrl = imageNode?.GetAttributeValue("content", null),
                Difficulty = Difficulty.Mittel,
                CookingTimeMinutes = 30, // Default
                Servings = 4, // Default
            };
        }

        return null;
    }

    private bool IsPrivateIpAddress(System.Net.IPAddress ipAddress)
    {
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();

            // 10.0.0.0/8
            if (ipBytes[0] == 10) return true;

            // 172.16.0.0/12
            if (ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31) return true;

            // 192.168.0.0/16
            if (ipBytes[0] == 192 && ipBytes[1] == 168) return true;

            // 169.254.0.0/16 (Link-local)
            if (ipBytes[0] == 169 && ipBytes[1] == 254) return true;
        }
        else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            if (ipAddress.IsIPv6SiteLocal || ipAddress.IsIPv6LinkLocal) return true;
        }

        return false;
    }
}
