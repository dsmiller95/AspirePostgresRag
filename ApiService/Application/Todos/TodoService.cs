using ApiService.Application.Ai;
using Data;
using Data.TodoItems;
using Domain.TodoItems;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ApiService.Application.Todos;

public interface ITodoService
{
    Task<List<TodoItem>> GetTodos();
    Task<TodoItem?> GetTodo(int id);
    Task<TodoItem> CreateTodo(CreateTodoItem todoItem);
    Task<TodoItem> UpdateTodo(int id, UpdateTodoItemCompleted updatedTodo);
    Task<bool> DeleteTodo(int id);
    
    Task<List<RankedTodoItem>> SearchTodos(string search);
}

public class TodoService(AppDbContext appDb, IEmbeddingService embeddingService) : ITodoService
{
    public async Task<List<TodoItem>> GetTodos()
    {
        return await appDb.TodoItems.Select(x => x.ToDomain()).ToListAsync();
    }

    public async Task<TodoItem?> GetTodo(int id)
    {
        var item = await appDb.TodoItems.FindAsync(id);
        return item?.ToDomain();
    }

    public async Task<TodoItem> CreateTodo(CreateTodoItem todoItem)
    {
        var embedding = await embeddingService.GetEmbeddingAsync(todoItem.Title);
        
        var dbItem = new TodoDbItem
        {
            Title = todoItem.Title,
            IsCompleted = todoItem.IsCompleted,
            Embedding = new Vector(embedding),
        };
        var added = appDb.TodoItems.Add(dbItem);
        await appDb.SaveChangesAsync();
        return added.Entity.ToDomain();
    }

    public async Task<TodoItem> UpdateTodo(int id, UpdateTodoItemCompleted request)
    {
        var existingItem = await appDb.TodoItems.FindAsync(id);
        if (existingItem is null)
        {
            throw new KeyNotFoundException($"Todo item with id {id} not found.");
        }

        var item = existingItem.ToDomain();
        item = item with
        {
            IsCompleted = request.IsCompleted,
        };
        
        var dbItem = TodoDbItem.From(item, existingItem.Embedding);
        appDb.TodoItems.Update(dbItem);
        await appDb.SaveChangesAsync();
        return dbItem.ToDomain();
    }

    public async Task<bool> DeleteTodo(int id)
    {
        var item = await appDb.TodoItems.FindAsync(id);
        if (item is null)
        {
            return false;
        }

        appDb.TodoItems.Remove(item);
        await appDb.SaveChangesAsync();
        return true;
    }

    public async Task<List<RankedTodoItem>> SearchTodos(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return (await GetTodos())
                .Select(x => RankedTodoItem.From(x, 0))
                .Take(10)
                .ToList();
        }
        var embedding = await embeddingService.GetEmbeddingAsync(search);
        var vector = new Vector(embedding);
        var results = await appDb.TodoItems
            .Select(x => new { DbItem = x, Distance = x.Embedding.L2Distance(vector) })
            .OrderBy(x => x.Distance)
            .Take(10)
            .Select(x => RankedTodoItem.From(x.DbItem.ToDomain(), x.Distance))
            .ToListAsync();
        
        return results;
    }
}
