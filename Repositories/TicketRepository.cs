using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private static List<Ticket> Tickets = new List<Ticket>();

        public void AddTicket(Ticket ticket)
        {
            ticket.Id = ToGuid(Tickets.Count + 1);
            Tickets.Add(ticket);
        }

        public List<Ticket> GetTicketsByClientId(Guid clientId)
        {
            return Tickets.Where(t => t.ClientId == clientId).ToList();
        }

        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}