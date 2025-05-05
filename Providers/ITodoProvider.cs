using TodoApp.Models;

namespace TodoApp.Providers;

public interface ITodoProvider
{
    Task<TodoResponse> GetTodosAsync(string? search = null);
    Task<TodoItem> GetByIdAsync(Guid id);
    Task<TodoItem> CreateAsync(TodoItem item);
    Task UpdateAsync(Guid id, TodoItem item);
    Task DeleteAsync(Guid id);
    Task<DateTime> GetLastUpdatedAsync();
}