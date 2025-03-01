namespace Ticket.API.Models;

public class TicketDto
{
    public string Id { get; set; }
    public string User { get; set; }
    public string CompanyId { get; set; }  //// Each ticket belongs to a company
    public string Status { get; set; }
}