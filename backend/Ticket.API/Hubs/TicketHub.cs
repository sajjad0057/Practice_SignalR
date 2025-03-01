using Microsoft.AspNetCore.SignalR;

namespace Ticket.API.Hubs;

public class TicketHub : Hub
{
    public async Task SendTicketUpdate(string ticketId, string user, string status)
    {
        await Clients.All.SendAsync("ReceiveTicketUpdate", ticketId, user, status);
    }
}
