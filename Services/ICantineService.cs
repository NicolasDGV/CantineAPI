using CantineAPI.Models;

namespace CantineAPI.Services
{
    public interface ICantineService
    {
        Client CreditAccount(int clientId, decimal montant);
        Ticket Pay(int clientId, List<int> repasIds);
        List<Ticket> GetTicketsByClientId(int clientId);
    }
}