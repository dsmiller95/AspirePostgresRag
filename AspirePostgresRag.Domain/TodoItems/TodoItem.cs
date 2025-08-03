namespace AspirePostgresRag.Models.TodoItems;

public record TodoItem
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required bool IsCompleted { get; init; }
}
