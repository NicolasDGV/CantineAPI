using CantineAPI.Models.Enums;

namespace CantineAPI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public decimal BudgetCantine { get; set; }
        public ClientType ClientType { get; set; }
    }
}
