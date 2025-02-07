using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private static List<Ticket> Tickets = new List<Ticket>();

        public void AddTicket(Ticket ticket)
        {
            ticket.Id = Tickets.Count + 1;
            Tickets.Add(ticket);
        }

        public List<Ticket> GetTicketsByClientId(int clientId)
        {
            return Tickets.Where(t => t.ClientId == clientId).ToList();
        }
    }
}