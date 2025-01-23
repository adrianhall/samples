using ClientApp.Interfaces;
using ClientApp.Models;
using CommunityToolkit.Datasync.Client;
using CommunityToolkit.Datasync.Client.Authentication;
using CommunityToolkit.Datasync.Client.Http;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using System.Diagnostics;

namespace ClientApp.Services;

public class OnlineTodoService : ITodoService
{
    private readonly DatasyncServiceClient<TodoItem> client;

    public OnlineTodoService(IPublicClientApplication identityClient)
    {
        Debug.WriteLine("OnlineTodoService created");
        var clientOptions = new HttpClientOptions()
        {
            Endpoint = new Uri(Constants.ServiceUri),
            HttpPipeline = [
                new GenericAuthenticationProvider(GetAuthenticationToken)
            ]
        };
        client = new(clientOptions);
        IdentityClient = identityClient;
    }

    public IPublicClientApplication IdentityClient { get; }

    public async Task<AuthenticationToken> GetAuthenticationToken(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine("GetAuthenticationToken called");
        var accounts = await IdentityClient.GetAccountsAsync();
        AuthenticationResult? result = null;
        try
        {
            result = await IdentityClient.AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault()).ExecuteAsync(cancellationToken);
        }
        catch (MsalUiRequiredException)
        {
            result = await IdentityClient
                .AcquireTokenInteractive(Constants.Scopes)
                .WithUseEmbeddedWebView(true)
                .ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: Authentication failed: {ex.Message}");
        }

        return new AuthenticationToken
        {
            DisplayName = result?.Account?.Username ?? "",
            ExpiresOn = result?.ExpiresOn ?? DateTimeOffset.MinValue,
            Token = result?.AccessToken ?? "",
            UserId = result?.Account?.Username ?? ""
        };
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
