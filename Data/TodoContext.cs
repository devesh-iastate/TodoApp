using Microsoft.EntityFrameworkCore;
using TodoApp.Models;
using System.Text.Json;

namespace TodoApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure one-to-many relationship
        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey("UserId")  // EF requires a FK name if not on model
            .OnDelete(DeleteBehavior.Cascade);

        // Store Dictionary<string, DateTime> as JSON
        modelBuilder.Entity<User>()
            .Property(u => u.LastUpdated)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<Dictionary<string, DateTime>>(v, new JsonSerializerOptions()));
    }
}