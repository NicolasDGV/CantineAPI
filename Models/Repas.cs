using CantineAPI.Models.Enums;

namespace CantineAPI.Models
{
    public class Repas
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public RepasType RepasType { get; set; }
    }
}
