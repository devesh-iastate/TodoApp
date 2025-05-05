using FireSharp.Interfaces;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Providers;

public static class TodoProviderFactory
{
    public static ITodoProvider Create(string providerName, AppDbContext context, string email)
    {
        return providerName switch
        {
            "LocalDb" => new DbTodoProvider(context, email),
            "FirebaseRealtimeDb" => new FirebaseTodoProvider(email),
            _ => throw new ArgumentException($"Unknown provider: {providerName}. Ensure the provider name is valid.")
        };
    }
}