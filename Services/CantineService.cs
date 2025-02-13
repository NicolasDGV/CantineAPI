using CantineAPI.Models;
using CantineAPI.Models.Enums;
using CantineAPI.Repositories;
using CantineAPI.Exceptions;
using CantineAPI.DTOs;

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

        public Client CreateClient(ClientDto dto)
        {
            var client = new Client
            {
                Id                  = Guid.NewGuid(),
                Name                = dto.Name,
                ClientType          = dto.Type,
                BudgetCantine       = dto.BudgetCantine
            };

            _clientRepository.Add(client);
            return client;
        }

        public Client? GetClientById(Guid id) => _clientRepository.GetClientById(id);


        public bool CreditAccount(Guid clientId, decimal amount)
        {
            var client = _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                return false;
            }

            client.BudgetCantine += amount;
            _clientRepository.UpdateClient(client);
            return true;
        }

        public Ticket Pay(Guid clientId, TicketDto ticketDto)
        {
            var client = _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                throw new NotFoundException("Client non trouvé");
            }

            var repasSelectionnes = _repasRepository.GetRepasByIds(ticketDto.Repas.Select(c => c.Id).ToList());

            bool fullMeal = TicketContainsFullMeal(repasSelectionnes);
            
            var total = fullMeal ? 10m : repasSelectionnes.Sum(r => r.Price);
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

        public List<Ticket> GetTicketsByClientId(Guid clientId)
        {
            return _ticketRepository.GetTicketsByClientId(clientId);
        }

        public List<Client> GetAllClient()
        {
            return _clientRepository.GetAllClient();
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