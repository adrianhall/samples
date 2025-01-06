using ClientApp.Interfaces;
using ClientApp.Models;
using CommunityToolkit.Datasync.Client;
using CommunityToolkit.Datasync.Client.Http;

namespace ClientApp.Services;

public class OnlineTodoService : ITodoService
{
    private const string baseUrl = "https://localhost:7181";
    private readonly DatasyncServiceClient<TodoItem> client;

    public OnlineTodoService()
    {
        var clientOptions = new HttpClientOptions()
        {
            Endpoint = new Uri(baseUrl)
        };
        client = new(clientOptions);
    }

    public async Task<TodoItem> AddTodoItemAsync(string title, CancellationToken cancellationToken = default)
    {
        ServiceResponse<TodoItem> response = await client.AddAsync(new TodoItem { Title = title }, cancellationToken);
        if (response.IsSuccessful && response.HasValue)
        {
            return response.Value!;
        }
        throw new ApplicationException(response.ReasonPhrase);
    }

    public async Task<List<TodoItem>> GetAllTodoItemsAsync(CancellationToken cancellationToken = default)
    {
        var list = await client.ToListAsync(cancellationToken);
        return list;
    }

    public async Task<TodoItem?> GetTodoItemAsync(string id, CancellationToken cancellationToken = default)
    {
        ServiceResponse<TodoItem> response = await client.GetAsync(id, cancellationToken);
        if (response.IsSuccessful && response.HasValue)
        {
            return response.Value!;
        }
        throw new ApplicationException(response.ReasonPhrase);
    }

    public async Task<TodoItem> ReplaceTodoItemAsync(TodoItem updatedItem, CancellationToken cancellationToken = default)
    {
        ServiceResponse<TodoItem> response = await client.ReplaceAsync(updatedItem, cancellationToken);
        if (response.IsSuccessful && response.HasValue)
        {
            return response.Value!;
        }
        throw new ApplicationException(response.ReasonPhrase);
    }
}
