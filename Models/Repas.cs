using CantineAPI.Models.Enums;

namespace CantineAPI.Models
{
    public class Repas
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public RepasType RepasType { get; set; }
    }
}
