using CantineAPI.Models;
using CantineAPI.DTOs;

namespace CantineAPI.Services
{
    public interface ICantineService
    {
        public Client CreateClient(ClientDto dto);
        public Client? GetClientById(Guid id);
        bool CreditAccount(Guid clientId, decimal montant);
        Ticket Pay(Guid clientId, TicketDto ticketDto);
        List<Ticket> GetTicketsByClientId(Guid clientId);
        public List<Client> GetAllClient();
    }
}