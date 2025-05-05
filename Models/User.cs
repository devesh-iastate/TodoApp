namespace TodoApp.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public Dictionary<string, DateTime>? LastUpdated { get; set; }
}