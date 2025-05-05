namespace TodoApp.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public User? User { get; set; }
}

public class TodoResponse
{
    public List<TodoItem> Todos { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}