using CantineAPI.Models;
using CantineAPI.Models.Enums;

namespace CantineAPI.DTOs;

public class TicketDto
{
    public List<Repas> Repas { get; set; } = new();
}