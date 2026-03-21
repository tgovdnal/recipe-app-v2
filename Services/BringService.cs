using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RecipeApp.Services;

public class BringService : IBringService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BringService> _logger;

    private string? _accessToken;
    private string? _listUuid;

    public BringService(HttpClient httpClient, IConfiguration configuration, ILogger<BringService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.getbring.com/rest/v2/");
    }

    public async Task<bool> AddItemsAsync(IEnumerable<string> items)
    {
        if (items == null || !items.Any()) return true;

        try
        {
            if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_listUuid))
            {
                if (!await LoginAsync())
                {
                    return false;
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            foreach (var item in items)
            {
                var payload = new
                {
                    itemId = item,
                    spec = "",
                    uuid = Guid.NewGuid().ToString()
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"bringlists/{_listUuid}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to add item '{item}' to Bring! list. Status: {response.StatusCode}");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding items to Bring! list");
            return false;
        }
    }

    private async Task<bool> LoginAsync()
    {
        var email = _configuration["BringApi:Email"];
        var password = _configuration["BringApi:Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("Bring API credentials are not configured.");
            return false;
        }

        var payload = new
        {
            email = email,
            password = password
        };

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("bringauth", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                _accessToken = authResult.GetProperty("access_token").GetString();
                _listUuid = authResult.GetProperty("bringListUUID").GetString();

                return !string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_listUuid);
            }

            _logger.LogWarning($"Failed to authenticate with Bring! API. Status: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Bring! API authentication");
            return false;
        }
    }
}
