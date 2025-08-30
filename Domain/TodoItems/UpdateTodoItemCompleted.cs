namespace Domain.TodoItems;

public record UpdateTodoItemCompleted(bool IsCompleted) : IHaveExample
{
    public static object GetExample()
    {
        return new UpdateTodoItemCompleted(true);
    }
}
