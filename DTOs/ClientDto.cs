using System.Text.Json.Serialization;
using CantineAPI.Models.Enums;
namespace CantineAPI.DTOs;

public class ClientDto
{
    public string Name { get; set; } = string.Empty;
    public ClientType Type { get; set; }
    public decimal BudgetCantine { get; set; }
}