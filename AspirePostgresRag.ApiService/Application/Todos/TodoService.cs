using AspirePostgresRag.Data;
using AspirePostgresRag.Models.TodoItems;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgresRag.ApiService.Application.Todos;

public interface ITodoService
{
    Task<List<TodoItem>> GetTodos();
    Task<TodoItem?> GetTodo(int id);
    Task<TodoItem> CreateTodo(CreateTodoItem todoItem);
    Task<TodoItem> UpdateTodo(int id, UpdateTodoItem updatedTodo);
    Task<bool> DeleteTodo(int id);
    
    Task<List<TodoItem>> SearchTodos(string search);
}

public class TodoService(TodoDbContext db) : ITodoService
{
    public async Task<List<TodoItem>> GetTodos()
    {
        return await db.TodoItems.Select(x => x.ToDomain()).ToListAsync();
    }

    public async Task<TodoItem?> GetTodo(int id)
    {
        var item = await db.TodoItems.FindAsync(id);
        return item?.ToDomain();
    }

    public async Task<TodoItem> CreateTodo(CreateTodoItem todoItem)
    {
        var dbItem = new TodoDbItem
        {
            Title = todoItem.Title,
            IsCompleted = todoItem.IsCompleted
        };
        var added = db.TodoItems.Add(dbItem);
        await db.SaveChangesAsync();
        return added.Entity.ToDomain();
    }

    public async Task<TodoItem> UpdateTodo(int id, UpdateTodoItem updatedTodo)
    {
        var existingItem = await db.TodoItems.FindAsync(id);
        if (existingItem is null)
        {
            throw new KeyNotFoundException($"Todo item with id {id} not found.");
        }

        var item = existingItem.ToDomain();
        item = item with
        {
            Title = updatedTodo.Title,
            IsCompleted = updatedTodo.IsCompleted,
        };
        
        var dbItem = TodoDbItem.FromDomain(item);
        db.TodoItems.Update(dbItem);
        await db.SaveChangesAsync();
        return dbItem.ToDomain();
    }

    public async Task<bool> DeleteTodo(int id)
    {
        var item = await db.TodoItems.FindAsync(id);
        if (item is null)
        {
            return false;
        }

        db.TodoItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<TodoItem>> SearchTodos(string search)
    {
        throw new NotImplementedException();
    }
}
