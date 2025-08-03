﻿using AspirePostgresRag.Models.TodoItems;
using Pgvector;

namespace AspirePostgresRag.Data;

public class TodoDbItem
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required bool IsCompleted { get; init; } 
    
    /// <summary>
    /// length of 1536, openai text-embedding-3-small model
    /// </summary>
    public required Vector Embedding { get; set; }
    
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
    
    public static TodoDbItem From(TodoItem item, Vector embedding)
    {
        return new TodoDbItem
        {
            Id = item.Id,
            Title = item.Title,
            IsCompleted = item.IsCompleted,
            Embedding = embedding,
        };
    }
}
