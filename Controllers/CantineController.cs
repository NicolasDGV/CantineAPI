using Microsoft.AspNetCore.Mvc;
using CantineAPI.Services;
using CantineAPI.Models;

namespace CantineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CantineController : ControllerBase
    {
        private readonly ICantineService _cantineService;

        public CantineController(ICantineService cantineService)
        {
            _cantineService = cantineService;
        }

        [HttpPost("crediter/{clientId}/{montant}")]
        public IActionResult CrediterCompte(int clientId, decimal montant)
        {
            var client = _cantineService.CreditAccount(clientId, montant);
            return Ok(client);
        }

        [HttpPost("payer/{clientId}")]
        public IActionResult PayerRepas(int clientId, [FromBody] List<int> repasIds)
        {
            var ticket = _cantineService.Pay(clientId, repasIds);
            return Ok(ticket);
        }

        [HttpGet("tickets/{clientId}")]
        public IActionResult GetTickets(int clientId)
        {
            var tickets = _cantineService.GetTicketsByClientId(clientId);
            return Ok(tickets);
        }
    }
}