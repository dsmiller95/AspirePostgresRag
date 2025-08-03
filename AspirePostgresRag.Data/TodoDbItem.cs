using AspirePostgresRag.Models.TodoItems;

namespace AspirePostgresRag.Data;

public class TodoDbItem
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required bool IsCompleted { get; init; } 
    
    public TodoItem ToDomain()
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(Id, 0);
        return new TodoItem
        {
            Id = Id,
            Title = Title,
            IsCompleted = IsCompleted
        };
    }
    
    public static TodoDbItem FromDomain(TodoItem item)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(item.Id, 0);
        return new TodoDbItem
        {
            Id = item.Id,
            Title = item.Title,
            IsCompleted = item.IsCompleted
        };
    }
}
