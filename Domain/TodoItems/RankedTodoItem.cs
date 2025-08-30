namespace Domain.TodoItems;

public record RankedTodoItem : TodoItem
{
    public required double Score { get; init; }
    
    public static RankedTodoItem From(TodoItem todoItem, double score) => new RankedTodoItem
    {
        Id = todoItem.Id,
        Title = todoItem.Title,
        IsCompleted = todoItem.IsCompleted,
        Score = score,
    };
}
