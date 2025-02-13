using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public class RepasRepository : IRepasRepository
    {
        private static List<Repas> RepasDisponibles = new List<Repas>
        {
            new Repas { Id = 1, Name = "Pizza", Price = 5.00m },
            new Repas { Id = 2, Name = "Salade", Price = 3.50m },
            new Repas { Id = 3, Name = "Dessert", Price = 2.00m }
        };

        public List<Repas> GetRepasByIds(List<int> repasIds)
        {
            return RepasDisponibles.Where(r => repasIds.Contains(r.Id)).ToList();
        }

        public IEnumerable<Repas> GetAll()
        {
            return RepasDisponibles.ToList();
        }

        public void Add(Repas repas)
        {
            RepasDisponibles.Add(repas);
        }

        public void Remove(int id)
        {
            var repas = GetRepasByIds(new List<int>() {id}).FirstOrDefault();
            if (repas != null)
            {
                RepasDisponibles.Remove(repas);
            }
        }
    }
}