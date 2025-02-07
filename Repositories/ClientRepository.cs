using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private static List<Client> Clients = new List<Client>
        {
            new Client { Id = 1, Nom = "Jean Dupont", BudgetCantine = 100.00m }
        };

        public Client GetClientById(int clientId)
        {
            return Clients.FirstOrDefault(c => c.Id == clientId);
        }

        public List<Client> GetAllClient()
        {
            return Clients.ToList();
        }

        public virtual void Add(Client client) => Clients.Add(client);

        public void UpdateClient(Client client)
        {
            var existingClient = Clients.FirstOrDefault(c => c.Id == client.Id);
            if (existingClient != null)
            {
                existingClient.Nom = client.Nom;
                existingClient.BudgetCantine = client.BudgetCantine;
            }
        }
    }
}