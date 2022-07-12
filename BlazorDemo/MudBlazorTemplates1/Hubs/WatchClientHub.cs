using Microsoft.AspNetCore.SignalR;

namespace MudBlazorTemplates1.Hubs;

public class WatchClientHub : Hub<IWatchClient>
{

    public override async Task OnConnectedAsync()
    {
        var watchClientInfo = await Clients.Caller.GetWatchClientInfo();
        await base.OnConnectedAsync();
    }
}