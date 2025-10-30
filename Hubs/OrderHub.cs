// This is a real-time messaging server 
// that broadcasts order updates to all connected clients instantly.
// It is used to notify users about new orders and updates.

// SignalR is a library that provides real-time web functionality.
// It allows the server to push content to the client instantly.

using Microsoft.AspNetCore.SignalR; // Import SignalR library

namespace DeliveryApp.Hubs;

public class OrderHub : Hub // Inherit from Hub class
{
    public async Task SendOrderUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveOrderUpdate", message);
    }
}
