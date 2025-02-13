using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public interface IClientRepository
    {
        Client GetClientById(Guid clientId);
        public List<Client> GetAllClient();
        void UpdateClient(Client client);
        void Add(Client client);
        
    }
}