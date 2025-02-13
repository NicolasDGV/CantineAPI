using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public interface ITicketRepository
    {
        void AddTicket(Ticket ticket);
        List<Ticket> GetTicketsByClientId(Guid clientId);
    }
}