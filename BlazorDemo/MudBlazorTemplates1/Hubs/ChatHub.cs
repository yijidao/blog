﻿using Microsoft.AspNetCore.SignalR;

namespace MudBlazorTemplates1.Hubs;

public class ChatHub : Hub
{



    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}