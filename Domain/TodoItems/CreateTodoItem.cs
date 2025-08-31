namespace Domain.TodoItems;

public record CreateTodoItem(string Title, bool IsCompleted) : IHaveExample
{
    public static object GetExample() => 
        new CreateTodoItem("Buy groceries", false);
}
