using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Providers;

public class DbTodoProvider(AppDbContext context, string email) : ITodoProvider
{
    private const string ProviderKey = "LocalDb";
    public async Task<TodoResponse> GetTodosAsync(string? search)
    {
        var user = await GetOrCreateUserAsync();
    
        var query = context.TodoItems
            .Include(t => t.User)
            .Where(t => t.User.Email == email);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t =>
                t.Title.Contains(search) || t.Description.Contains(search));
        }

        var todos = await query.ToListAsync();
        var timestamp = user.LastUpdated?.GetValueOrDefault(ProviderKey) ?? DateTime.MinValue;

        return new TodoResponse
        {
            Todos = todos,
            LastUpdated = timestamp
        };
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        var todo = await context.TodoItems.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id && t.User.Email == email);
        return todo;
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        var user = await GetOrCreateUserAsync();
        item.Id = Guid.NewGuid();
        item.User = user;

        context.TodoItems.Add(item);
        await context.SaveChangesAsync();
        await UpdateLastUpdatedAsync(user, ProviderKey);
        return item;
    }

    public async Task UpdateAsync(Guid id, TodoItem item)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Todo not found or not owned by user.");

        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.IsComplete = item.IsComplete;
        await context.SaveChangesAsync();
        
        var user = await context.Users.FirstAsync(u => u.Email == email);
        await UpdateLastUpdatedAsync(user, ProviderKey);
    }

    public async Task DeleteAsync(Guid id)
    {
        var todo = await GetByIdAsync(id);
        if (todo == null) return;

        context.TodoItems.Remove(todo);
        await context.SaveChangesAsync();
        var user = await context.Users.FirstAsync(u => u.Email == email);
        await UpdateLastUpdatedAsync(user, ProviderKey);
    }

    private async Task<User> GetOrCreateUserAsync()
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null) return user;
        user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            LastUpdated = new Dictionary<string, DateTime>()
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    
    private async Task UpdateLastUpdatedAsync(User user, string provider)
    {
        user.LastUpdated ??= new Dictionary<string, DateTime>();

        user.LastUpdated[provider] = DateTime.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
    
    public async Task<DateTime> GetLastUpdatedAsync()
    {
        var user = await GetOrCreateUserAsync();
        var timestamp = user.LastUpdated?.GetValueOrDefault(ProviderKey) ?? DateTime.MinValue;
        return timestamp;
    }
}