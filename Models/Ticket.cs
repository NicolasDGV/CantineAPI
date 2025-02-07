namespace CantineAPI.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientType { get; set; } = string.Empty;
        public List<Repas> Repas { get; set; }
        public decimal Total { get; set; }
        public decimal Reduction { get; set; }
        public decimal TotalFinal { get; set; }
    }
}