using Microsoft.AspNetCore.Mvc;
using CantineAPI.Services;
using CantineAPI.Models;
using CantineAPI.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace CantineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CantineController : ControllerBase
    {
        private readonly ICantineService _cantineService;

        public CantineController(ICantineService cantineService)
        {
            _cantineService = cantineService;
        }

        [HttpPost]
        public IActionResult AjouterClient([FromBody] ClientDto clientDto)
        {
            var client = _cantineService.CreateClient(clientDto);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(Guid id)
        {
            var client = _cantineService.GetClientById(id);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpGet]
        public IActionResult GetTousLesClients()
        {
            var clients = _cantineService.GetAllClient();
            return Ok(clients);
        }

        [HttpPost("crediter/{clientId}/{montant}")]
        public IActionResult CrediterCompte(Guid clientId, decimal montant)
        {
            var client = _cantineService.CreditAccount(clientId, montant);
            return Ok(client);
        }

        [HttpPost("payer/{clientId}")]
        public IActionResult Pay(Guid clientId, [FromBody] TicketDto ticket)
        {
            var ticket = _cantineService.Pay(clientId, repasIds);
            return Ok(ticket);
        }

        [HttpGet("tickets/{clientId}")]
        public IActionResult GetTickets(Guid clientId)
        {
            var tickets = _cantineService.GetTicketsByClientId(clientId);
            return Ok(tickets);
        }
    }
}