using CommunityToolkit.Datasync.Server;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace TodoApp.Service.Controllers;

public class ServiceHub(IDatasyncServiceOptions options) : Hub
{
    public async Task SendMessage(RepositoryUpdatedEventArgs eventArgs)
    {
        string jsonContent = JsonSerializer.Serialize(eventArgs, options.JsonSerializerOptions);
        await Clients.All.SendAsync("ServiceChange", jsonContent);
    }
}
