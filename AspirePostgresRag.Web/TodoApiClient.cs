using AspirePostgresRag.Models.TodoItems;

namespace AspirePostgresRag.Web;

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
    
    public async Task<TodoItem?> Create(CreateTodoItem item, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/todos", item, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TodoItem>(cancellationToken: cancellationToken);
        }
        return null;
    }
    
    public async Task<bool> Update(int id, UpdateTodoItem item, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/todos/{id}", item, cancellationToken);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/todos/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
