namespace AspirePostgresRag.Models.TodoItems;

public static class TodoBogus
{
    public static List<TodoItem> Generate(int count = 10)
    {
        var faker = new Bogus.Faker<TodoItem>()
            .RuleFor(t => t.Id, f => 0) // Id is set by the database
            .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(t => t.IsCompleted, f => f.Random.Bool(0.3f)); // 30% chance to be completed

        return faker.Generate(count);
    }
}
