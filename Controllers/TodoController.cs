using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Providers;

namespace TodoApp.Controllers;

[ApiController]
[Authorize]
[Route("api/todos/{provider}")]
public class TodoController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private string GetEmail() =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? throw new UnauthorizedAccessException("Email claim not found");

    [HttpGet]
    public async Task<ActionResult<TodoResponse>> GetTodos(string provider, [FromQuery] string? search = null)
    {
        try
        {
            var email = GetEmail();
            var service = TodoProviderFactory.Create(provider, context, email);
            var todos = await service.GetTodosAsync(search);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTodos: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(string provider, Guid id)
    {
        var email = GetEmail();
        var service = TodoProviderFactory.Create(provider, context, email);
        var todo = await service.GetByIdAsync(id);
        return todo != null ? Ok(todo) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo(string provider, TodoItem item)
    {
        var email = GetEmail();
        var service = TodoProviderFactory.Create(provider, context, email);
        var created = await service.CreateAsync(item);
        return CreatedAtAction(nameof(GetTodo), new { provider = provider, id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(string provider, Guid id, TodoItem item)
    {
        if (id != item.Id) return BadRequest();
        var email = GetEmail();
        var service = TodoProviderFactory.Create(provider, context, email);
        await service.UpdateAsync(id, item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(string provider, Guid id)
    {
        var email = GetEmail();
        var service = TodoProviderFactory.Create(provider, context, email);
        await service.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("last-updated")]
    public async Task<ActionResult<DateTime>> GetLastUpdated(string provider)
    {
        var email = GetEmail();
        var service = TodoProviderFactory.Create(provider, context, email);
        var time = await service.GetLastUpdatedAsync();
        return Ok(time);
    }
}