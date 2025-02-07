using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public interface IClientRepository
    {
        Client GetClientById(int clientId);
        void UpdateClient(Client client);
    }
}