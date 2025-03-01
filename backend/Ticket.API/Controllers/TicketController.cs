using Ticket.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ticket.API.Hubs;
using System.Text.Json;


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

        //// if want to send notification or ticket booking info just matching companies that connected
        await _hubContext.Clients.Group(ticket.CompanyId)
            .SendAsync("ReceiveTicketUpdate", ticket.Id, ticket.User, ticket.Status, ticket.CompanyId);


        //// if want to send notification or ticket booking info all connected companies
        //await _hubContext.Clients.All
        //    .SendAsync("ReceiveTicketUpdate", ticket.Id, ticket.User, ticket.Status, ticket.CompanyId);

        Console.WriteLine($"[TicketController.BookTicket] -> BookTicket : {JsonSerializer.Serialize(_tickets)}");


        return Ok(ticket);
    }

    [HttpGet]
    public IActionResult GetTickets() => Ok(_tickets);
}
