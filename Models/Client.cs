using CantineAPI.Models.Enums;

namespace CantineAPI.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal BudgetCantine { get; set; }
        public ClientType ClientType { get; set; }
    }
}
