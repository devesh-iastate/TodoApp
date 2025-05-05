using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using TodoApp.Models;
using DotNetEnv;

namespace TodoApp.Providers;

public class FirebaseTodoProvider(string email) : ITodoProvider
{
    private const string ProviderKey = "FirebaseRealtimeDb";
    private readonly IFirebaseClient _client = CreateFirebaseClient();

    private static IFirebaseClient CreateFirebaseClient()
    {
        Env.Load();

        var config = new FirebaseConfig
        {
            BasePath = Environment.GetEnvironmentVariable("FIREBASE_DB_URL")!,
            AuthSecret = Environment.GetEnvironmentVariable("FIREBASE_DB_SECRET")!
        };

        return new FirebaseClient(config);
    }

    private string GetUserPath() => $"users/{email.Replace(".", "_")}";

    public async Task<TodoResponse> GetTodosAsync(string? search = null)
    {
        try
        {
            var path = $"{GetUserPath()}/todos";
            var response = await _client.GetAsync(path);
            var raw = response.Body;

            Dictionary<string, TodoItem>? data = null;

            if (!string.IsNullOrWhiteSpace(raw) && raw != "null" && raw.StartsWith("{"))
            {
                data = response.ResultAs<Dictionary<string, TodoItem>>();
            }

            var todos = data?.Values
                .Where(t => string.IsNullOrEmpty(search)
                            || (t.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                            || (t.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList() ?? [];

            var timestampResponse = await _client.GetAsync($"{GetUserPath()}/lastUpdated/{ProviderKey}");
            var timestampRaw = timestampResponse.Body;

            DateTime timestamp = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(timestampRaw) && timestampRaw != "null" && DateTime.TryParse(timestampRaw.Trim('"'), out var parsed))
            {
                timestamp = parsed;
            }

            return new TodoResponse
            {
                Todos = todos,
                LastUpdated = timestamp
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Error in Firebase GetTodosAsync: {ex}");
            throw new Exception("Failed to fetch todos from Firebase", ex);
        }
    }

    public async Task<TodoItem> GetByIdAsync(Guid id)
    {
        var response = await _client.GetAsync($"{GetUserPath()}/todos/{id}");
        return response.ResultAs<TodoItem>()!;
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        item.Id = Guid.NewGuid();
        await _client.SetAsync($"{GetUserPath()}/todos/{item.Id}", item);
        await UpdateLastUpdatedAsync();
        return item;
    }

    public async Task UpdateAsync(Guid id, TodoItem item)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Todo not found.");
        item.Id = id;
        await _client.UpdateAsync($"{GetUserPath()}/todos/{id}", item);
        await UpdateLastUpdatedAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _client.DeleteAsync($"{GetUserPath()}/todos/{id}");
        await UpdateLastUpdatedAsync();
    }

    public async Task<DateTime> GetLastUpdatedAsync()
    {
        try
        {
            var response = await _client.GetAsync($"{GetUserPath()}/lastUpdated/{ProviderKey}");

            // This gets the raw string body from Firebase
            var raw = response.Body;

            // If Firebase returned "null" or an empty string
            if (string.IsNullOrWhiteSpace(raw) || raw == "null")
                return DateTime.MinValue;

            // Trim quotes and parse safely
            if (DateTime.TryParse(raw.Trim('"'), out var parsed))
                return parsed;

            return DateTime.MinValue;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Firebase GetLastUpdatedAsync: {ex.Message}");
            return DateTime.MinValue;
        }
    }

    private async Task UpdateLastUpdatedAsync()
    {
        var timestamp = DateTime.UtcNow;
        await _client.SetAsync($"{GetUserPath()}/lastUpdated/{ProviderKey}", timestamp);
    }
}
