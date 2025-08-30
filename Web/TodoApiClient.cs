using Domain.TodoItems;

namespace Web;

public class TodoApiClient(HttpClient httpClient)
{
    public async Task<TodoItem[]> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await httpClient.GetFromJsonAsync<List<TodoItem>>("/todos", cancellationToken);
        return items?.ToArray() ?? [];
    }
    
    public async Task<TodoItem?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<TodoItem>($"/todos/{id}", cancellationToken);
    }
    
    public async Task<RankedTodoItem[]> Search(string search, CancellationToken cancellationToken = default)
    {
        var items = await httpClient.GetFromJsonAsync<List<RankedTodoItem>>($"/todos/search?query={Uri.EscapeDataString(search)}", cancellationToken);
        return items?.ToArray() ?? [];
    }
    
    public async Task<TodoItem?> Create(CreateTodoItem item, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/todos", item, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<TodoItem>(cancellationToken: cancellationToken);
    }
    
    public async Task<TodoItem?> UpdateCompleted(int id, UpdateTodoItemCompleted itemCompleted, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/todos/{id}", itemCompleted, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TodoItem>(cancellationToken: cancellationToken);
    }
    
    public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/todos/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
