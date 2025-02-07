using CantineAPI.Models;
using CantineAPI.Models.Enums;
using CantineAPI.Repositories;
using CantineAPI.Exceptions;

namespace CantineAPI.Services
{
    public class CantineService : ICantineService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IRepasRepository _repasRepository;
        private readonly ITicketRepository _ticketRepository;

        public CantineService(IClientRepository clientRepository, IRepasRepository repasRepository, ITicketRepository ticketRepository)
        {
            _clientRepository = clientRepository;
            _repasRepository = repasRepository;
            _ticketRepository = ticketRepository;
        }

        public Client CreditAccount(int clientId, decimal montant)
        {
            var client = _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                throw new NotFoundException("Client non trouvé");
            }

            client.BudgetCantine += montant;
            _clientRepository.UpdateClient(client);
            return client;
        }

        public Ticket Pay(int clientId, List<int> repasIds)
        {
            var client = _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                throw new NotFoundException("Client non trouvé");
            }

            var repasSelectionnes = _repasRepository.GetRepasByIds(repasIds);

            bool fullMeal = TicketContainsFullMeal(repasSelectionnes);
            
            var total = fullMeal ? 10m : repasSelectionnes.Sum(r => r.Prix);
            var reduction = GetReduction(client, total);

            if (client.BudgetCantine < total)
            {
                throw new InsufficientBudgetException("Solde insuffisant");
            }

            client.BudgetCantine -= total;
            _clientRepository.UpdateClient(client);

            var ticket = new Ticket
            {
                ClientId = clientId,
                Date = DateTime.Now,
                Repas = repasSelectionnes,
                Total = total
            };

            _ticketRepository.AddTicket(ticket);
            return ticket;
        }

        public List<Ticket> GetTicketsByClientId(int clientId)
        {
            return _ticketRepository.GetTicketsByClientId(clientId);
        }

        private static decimal GetReduction(Client client, decimal total)
        {
            return client.ClientType switch
            {
                ClientType.Interne => 7.5m,
                ClientType.Prestataire => 6m,
                ClientType.VIP => total,
                ClientType.Stagiaire => 10m,
                ClientType.Visiteur => 0m,
                _ => 0m
            };
        }

        private static bool TicketContainsFullMeal(List<Repas> repas)
        {
            return repas.Any(p => p.RepasType == RepasType.Entree) &&
                repas.Any(p => p.RepasType == RepasType.Plat) &&
                repas.Any(p => p.RepasType == RepasType.Dessert) &&
                repas.Any(p => p.RepasType == RepasType.Pain);
        }
    }
}