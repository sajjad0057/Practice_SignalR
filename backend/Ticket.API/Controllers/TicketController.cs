using Ticket.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ticket.API.Hubs;


namespace Ticket.API.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController : ControllerBase
{
    private readonly IHubContext<TicketHub> _hubContext;
    private static List<TicketDto> _tickets = new();

    public TicketController(IHubContext<TicketHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookTicket([FromBody] TicketDto ticket)
    {
        _tickets.Add(ticket);
        await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", ticket.Id, ticket.User, ticket.Status);
        return Ok(ticket);
    }

    [HttpGet]
    public IActionResult GetTickets() => Ok(_tickets);
}
