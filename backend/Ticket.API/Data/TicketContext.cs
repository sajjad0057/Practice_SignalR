using Microsoft.EntityFrameworkCore;
using Ticket.API.Models;

namespace Ticket.API.Data;

public class TicketContext : DbContext
{
    public DbSet<TicketDto> Tickets { get; set; } = default!;

    public TicketContext(DbContextOptions<TicketContext> options)
        : base(options)
    {
    }
}