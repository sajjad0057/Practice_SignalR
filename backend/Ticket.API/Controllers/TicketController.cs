using Ticket.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ticket.API.Hubs;
using System.Text.Json;
using Ticket.API.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;


namespace Ticket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TicketController : ControllerBase
{
    private readonly IHubContext<TicketHub> _hubContext;
    private readonly TicketContext _context;
    private readonly IDistributedCache _cache;

    public TicketController(IHubContext<TicketHub> hubContext, TicketContext context,IDistributedCache cache)   
    {
        _hubContext = hubContext;
        _context = context;
        _cache = cache;
    }


    [HttpPost("book")]
    public async Task<IActionResult> BookTicket([FromBody] TicketDto ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        await _cache.SetStringAsync(ticket.Id, JsonSerializer.Serialize(ticket));
        //// if want to send notification or ticket booking info just matching companies that connected
        await _hubContext.Clients.Group(ticket.CompanyId)
            .SendAsync("ReceiveTicketUpdate", ticket.Id, ticket.User, ticket.Status, ticket.CompanyId);


        //// if want to send notification or ticket booking info all connected companies
        //await _hubContext.Clients.All
        //    .SendAsync("ReceiveTicketUpdate", ticket.Id, ticket.User, ticket.Status, ticket.CompanyId);


        return Ok(ticket);
    }


    [HttpGet("All")]
    public async Task<IActionResult> GetAllTickets()
    {
        var tikets = await _context.Tickets.ToListAsync();
        return Ok(tikets);
    }


    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketsById(string ticketId)
    {
        var cacheTicket = await _cache.GetStringAsync(ticketId);

        if(!string.IsNullOrWhiteSpace(cacheTicket))
            return Ok(JsonSerializer.Deserialize<TicketDto>(cacheTicket));

        var ticket = await _context.Tickets.FindAsync(ticketId);

        if (ticket is not null)
        {
            await _cache.SetStringAsync(ticketId, JsonSerializer.Serialize(ticket));
            return Ok(ticket);
        }
            
        return NoContent();
    }
}
